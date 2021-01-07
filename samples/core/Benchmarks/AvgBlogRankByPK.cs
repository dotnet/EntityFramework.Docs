// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Benchmarks
{
    internal delegate double CallMe();              // DEBUG signature used by methods and DIYMark

    [MemoryDiagnoser]
    /// <summary>tests all read-only, so 1-off Setup suffices [no transaction rollback needed after each test]</summary>
    /// <remarks>
    /// 1   various methods here attempt to contrast Find[EF], First[LINQ] and other List/Dictionary approaches [hint: best="it depends"!]
    /// 2   EF Local offers some client-side caching (unless disabled implicitly for projections or explicitly by AsNoTracking)
    /// 3   if using a Local/List/Dictionary cache scheme code will benefit from cache-hit if pre-loaded ["warm"] or on pass# > 0
    /// 4   nominal params NumBlogs=1000 and NumPasses=1 (so cache overhead yields no actual benefit), but vary params to see effects!
    /// </remarks>
    public class AvgBlogRankByPK
    {
        [Params(500, 2000)]
        public int NumBlogs = 1000;       // number of records to write [once], and read [each pass]

        [Params(1, 2)]
        public int NumPasses = 1;         // number of read passes [>1 means may re-read previously-read warm data]

        private readonly string _sqlConnString;
        private readonly SqlConnection sqlCon;
        private int[] keysplat;                     // randomised 0 .. n-1 set of BlogId Primary Keys (i.e. 0 .. 999)

#if DEBUG
        private readonly CallMe[] methods = null;
#endif
        public AvgBlogRankByPK()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
            _sqlConnString = config.GetConnectionString("DefaultSql");
            sqlCon = new SqlConnection(_sqlConnString);         // will be used by ADO not EF

#if DEBUG
            var t = typeof(AvgBlogRankByPK);
            //foreach (var m in t.GetMethods().Where(m2 => m2.CustomAttributes.Any(a => a.AttributeType.Name == "BenchmarkAttribute")))
            //{
            //    Console.WriteLine("{1}\t{0}", m.Name, m.CustomAttributes.Count());
            //    var yyy = CallMe.CreateDelegate(typeof(CallMe), this, m.Name);
            //}
            methods = t.GetMethods()
                        .Where(m2 => m2.CustomAttributes
                        .Any(a => a.AttributeType.Name == "BenchmarkAttribute"))
                        .Select(m => (CallMe)CallMe.CreateDelegate(typeof(CallMe), this, m.Name)).ToArray();
#endif
        }

#if DEBUG
        /// <summary>
        ///     this is a poor-man's DIY Benchmark whilst I debug the various EF methods
        /// </summary>
        /// <remarks>
        /// 1   run in Debug (Optimise=false) [contrary to AndreyAk]
        /// 2   attached to VS [ditto]
        /// 3   no warm-up, removal of outliers, batch runs & stats calcs etc [yup, wrong on all counts!]
        /// 4   after debug works 100%, switch to Release build, and #define BM so subsequent runs invoke Benchmark tool
        /// </remarks>
        public void DIYMark()
        {
            var sw = new Stopwatch();
            foreach (var proc in methods)
            {
                Setup();                        // do the 1-off initialisation
                sw.Restart();                   // reset the timer for current independent test
                _ = proc();                     // conduct specific EF test (1st pass may fill DbContext, then 2nd reuse ctx)
                sw.Stop();
                Console.WriteLine($"{proc.Method.Name,-20}\ttook\t{sw.Elapsed}");  // don't believe any timings!
            }
        }
#endif

        [GlobalSetup]
        public void Setup()
        {
            using var context = new BloggingContext(_sqlConnString);    // ctx for Setup only [each Benchmark method creates a fresh one]
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData(NumBlogs);
            keysplat = UnSort.Jumble(Blog.baseID, NumBlogs);            // generate a randomised deck (i.e. shuffle the cards)
            if (sqlCon.State != ConnectionState.Open)
            {
                sqlCon.Open();                                          // keep open [do not specify CommandBehavior.CloseConnection]
            }
        }

        #region LoadEntitiesNoTracking
        [Benchmark(Description = "LoadEntitiesNoTracking {req=P,row=P*N,col=all,keep=F}")]
        public double LoadEntitiesNoTracking()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                foreach (var blog in ctx.Blogs.AsNoTracking())  // stream whole fresh record [order is idempotent so no PK]
                {
                    sum += blog.Rating;             // extract one field then junk everything at client [not stored in Local or for CT]
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ProjectOnlyRanking
        [Benchmark(Description = "ProjectOnlyRanking {req=P,row=P*N,col=1,keep=F}")]
        public double ProjectOnlyRanking()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                foreach (var rating in ctx.Blogs.Select(b => b.Rating))     // stream Rating field only [order is idempotent so no PK]
                {
                    sum += rating;
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region CalculateInDatabase
        [Benchmark(Description = "CalculateInDatabase {req=P,row=P,col=1,keep=F}", Baseline = true)]
        public double CalculateInDatabase()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            for (var p = 0; p < NumPasses; p++)
            {
                rslt = ctx.Blogs.Average(b => b.Rating);
            }
            return rslt;
        }
        #endregion

        #region ADOwhole
        [Benchmark(Description = "ADOwhole {req=P,row=P*N,col=all,keep=F}")]
        public double ADOwhole()
        {
            var rslt = -1d;
            // all fields incl unused ugly [Image] payload. instead of the lazy "*" I enscribed all columns for dr.GetInt32(5)
            var SqlCmd = new SqlCommand("SELECT BlogId, Name, Url, Image, CreationTime, Rating FROM Blogs", sqlCon);     // so Rating is 6th
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                using (var dr = SqlCmd.ExecuteReader())     // [order is idempotent so no PK]
                {
                    while (dr.Read())                       // stream whole record {rows=all,cols=all}
                    {
                        sum += dr.GetInt32(5);              // Rating =6th column [cf. SELECT above]
                        count++;
                    }
                    foreach (Blog blog in dr)               // stream whole field {rows=all,cols=all}
                    {
                        sum += blog.Rating;
                        count++;
                    }
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ADORatingOnly
        [Benchmark(Description = "ADORatingOnly {{req=P,row=P*N,col=1,keep=F}")]
        public double ADORatingOnly()
        {
            var rslt = -1d;
            var SqlCmd = new SqlCommand("SELECT Rating FROM Blogs", sqlCon);     // single field only [no ugly payload]
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                using (var dr = SqlCmd.ExecuteReader())     // [order is idempotent so no PK]
                {
                    while (dr.Read())                       // stream single field {rows=all,cols=1}
                    {
                        sum += dr.GetInt32(0);
                        count++;
                    }
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ADOCalcByDb
        [Benchmark(Description = "ADOCalcByDb {req=P,row=P,col=1,keep=F} - SQL computes avg")]
        public double ADOCalcByDb()
        {
            var rslt = -1d;
            var SqlCmd = new SqlCommand("SELECT Avg(convert(float,Rating)) FROM Blogs", sqlCon);   // result only [one record, no ugly payload]
            for (var p = 0; p < NumPasses; p++)
            {
                rslt = (double)SqlCmd.ExecuteScalar();          // [order is idempotent so no PK]
            }
            return rslt;
        }
        #endregion

        #region ADORatingSequential
        [Benchmark(Description = "ADORatingSequential {req=P,row=P*N,col=1,keep=F}")]
        public double ADORatingSequential()
        {
            var rslt = -1d;
            var SqlCmd = new SqlCommand("SELECT Rating FROM Blogs where BlogId=@BlogId", sqlCon);   // all fields incl unused ugly [Image] payload
            var p1 = SqlCmd.Parameters.Add("@BlogId", SqlDbType.Int);
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < NumBlogs + Blog.baseID; PK++)
                {
                    p1.Value = PK;                                  // specify key we require
                    sum += (int)SqlCmd.ExecuteScalar();             // stream single field {rows=all,cols=1}
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ColdFindSequential
        /// <summary>
        ///     seek record via PK using Find EF DbSet operation
        /// </summary>
        /// <returns>entire single record ie all columns is pulled down over network during roundtrip</returns>
        /// <remarks>
        /// 1   code will search Local cache first [will fail on p=0 as we start cold]
        /// 2   comparison is O(n) operation because Local arranged as simple List and not as Dictionary/HashSet
        /// 3   EF will roundtrip to db each single record {cols=all,rows=1} ("rbar"!), and merge into Local [also AsNoTracking n/a]
        /// 4   pass p>0 repeats operation so step #1 succeeds with cache-hit so no further network+Local RAM overhead
        /// </remarks>
        [Benchmark(Description = "ColdFindSequential {seq=P*N,row=P*N,col=all,keep=T}")]
        public double ColdFindSequential()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // Find will pull&pop if needed (aka on-demand), cache starts cold so Find must roundtrip p=0 only
            for (var p = 0; p < NumPasses; p++)     // p=0 will populate ctx, p>0 will re-use content
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < NumBlogs + Blog.baseID; PK++)
                {
                    // try Local first [trying to avoid roundtrip], then MSSQL if unfound at client (i.e. p=0 only)
                    // entire Blog entity returned over the wire. Find is always tracked, so wastes 90% that isn't Rating
                    sum += ctx.Blogs.Find(PK).Rating;
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmFindSequential
        [Benchmark(Description = "WarmFindSequential {req=1,row=N,col=all,keep=F}")]
        public double WarmFindSequential()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // removed .AsNoTracking() as this would disable Local EF cache (not just the CT functionality)
            _ = ctx.Blogs.ToList();               // pull down all records, keep all fields in EF (i.e. start warm)
                                                  // Find will pull&pop if needed (aka on-demand), cache starts warm so Find will not roundtrip each time (unlike First)
            for (var p = 0; p < NumPasses; p++)     // ctx already pre-populated, so each pass will re-use content
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < NumBlogs + Blog.baseID; PK++)
                {
                    sum += ctx.Blogs.Find(PK).Rating;
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ColdFirstSequential
        [Benchmark(Description = "ColdFirstSequential {seq=N,row=N,col=all,keep=F}")]
        public double ColdFirstSequential()         // WarmFirstSequential omitted as First never searches Local but always goes to db
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // First will always roundtrip, cache cold throughout and ignored by First
            for (var p = 0; p < NumPasses; p++)     // p=0 will not populate ctx, so p>0 same perf
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < NumBlogs + Blog.baseID; PK++)
                {
                    sum += ctx.Blogs
                            //.AsNoTracking()               // projections always skip EF Local and CT effort
                            .Where(b => b.BlogId == PK)     // use EF to find particular record [O(n)]
                            .Select(b => b.Rating)          // project only the fields we need (i.e. minimise LAN traffic)
                            .First();                       // roundtrip to ensure always get latest data
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmObjSequential
        [Benchmark(Description = "WarmObjSequential {req=N,row=N,col=2,keep=T}")]
        public double WarmObjSequential()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            var appcache = ctx.Blogs
                            //.AsNoTracking()                           // projections always skip EF Local and CT effort
                            .Select(b => new { b.BlogId, b.Rating })    // pull down {cols=2,rows=all}
                            .ToList();                                  //  but just stash {key, value} in appcache (not Local)
            for (var p = 0; p < NumPasses; p++)                         // always re-use appcache, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < Blog.baseID + NumBlogs; PK++)
                {
                    sum += appcache.First(b => b.BlogId == PK).Rating;  // repeatable-read from appcache [possible staler than db] is O(n)
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmHybridSequential
        [Benchmark(Description = "WarmHybridSequential {req=N,row=N,col=all,keep=T}")]
        public double WarmHybridSequential()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // comment-out next line if you want to metric ColdLocalSequential
            _ = ctx.Blogs.ToList();                 // pull down {cols=all,rows=all}, into Local
            for (var p = 0; p < NumPasses; p++)     // always re-use appcache, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < Blog.baseID + NumBlogs; PK++)
                {
                    var blog = ctx.Blogs.Local.First(b => b.BlogId == PK)       // try client-side Local cache first [stale] ..
                                ?? ctx.Blogs.First(b => b.BlogId == PK);        // .. otherwise pop [latest] from db [NB all Blog fields]
                    sum += blog.Rating;             // repeatable-read from Local [possible staler than db]
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmDictSequential
        [Benchmark(Description = "WarmDictSequential {req=N,row=N,col=2,keep=T}")]
        public double WarmDictSequential()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            var appDict = ctx.Blogs
                            //.AsNoTracking()                               // projections always skip EF Local and CT effort
                            .Select(b => new { b.BlogId, b.Rating })        // pull down {cols=2,rows=all}
                            .ToDictionary(b => b.BlogId, b => b.Rating);    // but just stash {key, value} in appDict (not Local)
            for (var p = 0; p < NumPasses; p++)                             // always re-use appDict, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                for (var PK = Blog.baseID; PK < Blog.baseID + NumBlogs; PK++)
                {
                    sum += appDict[PK];                                     // lookup value by PK efficiently [= "O(1)" operation]
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ADORatingRandom
        [Benchmark(Description = "ADORatingRandom {rand=P*N,row=P*N,col=1,keep=F}")]
        public double ADORatingRandom()
        {
            var rslt = -1d;
            var SqlCmd = new SqlCommand("SELECT Rating FROM Blogs where BlogId=@BlogId", sqlCon);   // single field [no unused ugly Image payload]
            var p1 = SqlCmd.Parameters.Add("@BlogId", SqlDbType.Int);
            for (var p = 0; p < NumPasses; p++)
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)
                {
                    p1.Value = PK;                              // specify key we require [always cold, no local warm cache]
                    sum += (int)SqlCmd.ExecuteScalar();         // roundtrip single field
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ColdFindRandom
        /// <summary>
        ///     seek record via PK using Find EF DbSet operation
        /// </summary>
        /// <returns>entire single record ie all columns is pulled down over network during roundtrip</returns>
        /// <remarks>
        /// 1   code will search Local cache first [will fail on p=0 as we start cold]
        /// 2   comparison is O(n) operation because Local arranged as simple List and not as Dictionary/HashSet
        /// 3   EF will roundtrip to db each single record {cols=all,rows=1} ("rbar"!), and merge into Local [also AsNoTracking n/a]
        /// 4   pass p>0 repeats operation so step #1 succeeds with cache-hit so no further network+Local RAM overhead
        /// </remarks>
        [Benchmark(Description = "ColdFindRandom {rand=N,row=N,col=all,keep=T}")]
        public double ColdFindRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // Find will pull&pop if needed (aka on-demand), cache starts cold so Find must roundtrip p=0 only
            for (var p = 0; p < NumPasses; p++)     // p=0 will populate ctx, p>0 will re-use content
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)                // OLTP will often need to retrieve by [synthetic] key
                {
                    // try Local first [trying to avoid roundtrip], then MSSQL if unfound at client (i.e. p=0 only)
                    // entire Blog entity returned over the wire. Find is always tracked, so wastes 90% that isn't Rating
                    sum += ctx.Blogs.Find(PK).Rating;
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmFindRandom
        [Benchmark(Description = "WarmFindRandom {req=1, row=N, col=all, keep=T}")]
        public double WarmFindRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // removed .AsNoTracking() as this would disable Local EF cache (not just the CT functionality)
            _ = ctx.Blogs.ToList();                         // Find will pull down all records AOT, keeping all fields in EF (i.e. start warm)
                                                            //  cache starts warm so Find will not roundtrip each time (unlike First)
            for (var p = 0; p < NumPasses; p++)             // ctx already pre-populated, so each pass will re-use content
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)                // OLTP will often need to retrieve by [synthetic] key
                {
                    sum += ctx.Blogs.Find(PK).Rating;       // Find will search Local first, and would go to db if unfound
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region ColdFirstRandom
        [Benchmark(Description = "ColdFirstRandom {rand=1, row=N, col=all, keep=T}")]
        public double ColdFirstRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            // First will always roundtrip, cache cold throughout and ignored by First [hence no WarmFirstRandom example]
            for (var p = 0; p < NumPasses; p++)     // always roundtrip, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)                // OLTP will often need to retrieve by [synthetic] key
                {
                    sum += ctx.Blogs                        // First will never search Local first, but will always roundtrip to db
                                                            //.AsNoTracking()               // projections always skip EF Local and CT effort
                            .Where(b => b.BlogId == PK)     // use EF to find particular record [O(n)]
                            .Select(b => b.Rating)          // project only the fields we need (i.e. minimise LAN traffic)
                            .First();                       // roundtrip to ensure always get latest data
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmObjRandom
        [Benchmark(Description = "WarmObjRandom {req=1, row=N, col=2, keep=T}")]
        public double WarmObjRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);

            var appcache = ctx.Blogs
                            //.AsNoTracking()                           // projections always skip EF Local and CT effort
                            .Select(b => new { b.BlogId, b.Rating })    // pull down {cols=2,rows=all}
                            .ToList();                                  //  but just stash {key, value} in appcache (not Local)
            for (var p = 0; p < NumPasses; p++)                         // always re-use appcache, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)                            // OLTP will often need to retrieve by [synthetic] key
                {
                    sum += appcache.First(b => b.BlogId == PK).Rating;  // repeatable-read from appcache [possible staler than db] is O(n)
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmHybridRandom
        [Benchmark(Description = "WarmHybridRandom  {req=N,row=N,col=all,keep=T}")]
        public double WarmHybridRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            _ = ctx.Blogs.ToList();                 // pull down {cols=all,rows=all}, into Local
            for (var p = 0; p < NumPasses; p++)     // always re-use appcache, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)        // OLTP will often need to retrieve by [synthetic] key
                {
                    var blog = ctx.Blogs.Local.First(b => b.BlogId == PK)       // try client-side Local cache first [stale] ..
                                ?? ctx.Blogs.First(b => b.BlogId == PK);        // .. otherwise pop [latest] from db
                    sum += blog.Rating;             // repeatable-read from Local [possible staler than db]
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        #region WarmDictRandom
        [Benchmark(Description = "WarmDictRandom {req=N,row=N,col=2,keep=T}")]
        public double WarmDictRandom()
        {
            var rslt = -1d;
            using var ctx = new BloggingContext(_sqlConnString);
            var appDict = ctx.Blogs
                            //.AsNoTracking()                               // projections always skip EF Local and CT effort
                            .Select(b => new { b.BlogId, b.Rating })        // pull down {cols=2,rows=all}
                            .ToDictionary(b => b.BlogId, b => b.Rating);    // but just stash {key, value} in appDict (not Local)
            for (var p = 0; p < NumPasses; p++)                             // always re-use appDict, so same perf each pass
            {
                var sum = 0;
                var count = 0;
                foreach (var PK in keysplat)                                // OLTP will often need to retrieve by [synthetic] key
                {
                    sum += appDict[PK];                                     // lookup value by PK efficiently [= "O(1)" operation]
                    count++;
                }
                rslt = (double)sum / count;
            }
            return rslt;
        }
        #endregion

        public class BloggingContext : DbContext
        {
            private readonly string _sqlConnString;

            public DbSet<Blog> Blogs { get; set; }

            public BloggingContext(string sqlConnString) => _sqlConnString = sqlConnString;

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(_sqlConnString);

            /// <summary>
            ///     create population of Blog records
            /// </summary>
            /// <remarks>
            /// 1   NB BlogId ranges from 1 to 1000 as per IDENTITY(1,1)
            /// 2   so this is NOT zero-based
            /// 3   alt possible for app to generate explicit BlogId values
            /// </remarks>
            public void SeedData(int numblogs, int seed = 123456)
            {
                const int ImageMin = 0, ImageMax = 3_000;       // suppose Blog has a non-trivial payload like a .JPG that must be downloaded
                var r = new Random(seed);
                Blogs.AddRange(
                    Enumerable.Range(0, numblogs).Select(i => new Blog(r.Next(ImageMin, ImageMax))
                    {
                        //BlogId = i + Blog.baseID,           // IDENTITY(1,1) by MSSQL, i.e. BlogId is NOT zero-based
                        Name = $"Blog{i}",
                        Url = $"blog{i}.blogs.net",
                        CreationTime = new DateTime(2020, 1, 1),
                        Rating = i % 5
                    }));
                _ = SaveChanges();
            }
        }

        public class Blog
        {
            public Blog()
            { }
            public Blog(int imglen = 0) => Image = new byte[imglen];
            public const int baseID = 1;
            //[Key]       // EF supposes IDENTITY(1,1) in relational db (e.g. MSSQL)
            //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]    // default
            //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]        // alt if app generates #
            public int BlogId { get; set; }             // Primary Key [by EF convention], and MSSQL assigns # on INSERT
            public string Name { get; set; }
            public string Url { get; set; }
            public byte[] Image { get; set; }           // potentially large payload that will fry perf (especially roundtrip to db)
            public DateTime CreationTime { get; set; }
            public int Rating { get; set; }
        }
    }
}
