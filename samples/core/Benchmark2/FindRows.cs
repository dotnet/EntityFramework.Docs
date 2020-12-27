// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchSql;
using Microsoft.Extensions.Configuration;

namespace Benchmark2
{
    [MemoryDiagnoser]
    public class FindRows
    {
        [GlobalSetup]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //SetupSqlLite(config);
            //SetupSqlLiteInMem(config);
            BenchSql.ProviderSql.SetupSqlServer(config);
            //SetupSqlServerInMem(config);
        }
        /*
                #region SqlLite
                private static void SetupSqlLite(IConfiguration config) => throw new NotImplementedException();

                [Benchmark]
                public static double SqlLite()
                {
                    var sum = 0;
                    var count = 0;
                    using var ctx = new BloggingContext();
                    foreach (var blog in ctx.Blogs)
                    {
                        sum += blog.Rating;
                        count++;
                    }

                    return (double)sum / count;
                }
                #endregion

                #region SqlLiteInMem
                private static void SetupSqlLiteInMem(IConfiguration config) => throw new NotImplementedException();

                [Benchmark]
                public static double SqlLiteInMem()
                {
                    var sum = 0;
                    var count = 0;
                    using var ctx = new BloggingContext();
                    foreach (var blog in ctx.Blogs.AsNoTracking())
                    {
                        sum += blog.Rating;
                        count++;
                    }

                    return (double)sum / count;
                }
                #endregion
        */
        #region SqlServer
        [Benchmark]
        public double SqlServer()
        {
            var sum = 0;
            var count = 0;
            using var ctx = new BlogContextSQL();
            foreach (var rating in ctx.Blogs.Select(b => b.Rating))
            {
                sum += rating;
                count++;
            }

            return (double)sum / count;
        }
        #endregion
        /*
                #region SqlServerInMem
                private static void SetupSqlServerInMem(IConfiguration config) => throw new NotImplementedException();

                [Benchmark(Baseline = true)]
                public static double SqlServerInMem()
                {
                    using var ctx = new BloggingContext();
                    return ctx.Blogs.Average(b => b.Rating);
                }
                #endregion
        */
    }
}
