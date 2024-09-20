using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public static class HierarchicalPartitionKeysSample
{
    public static async Task UseHierarchicalPartitionKeys()
    {
        Console.WriteLine($">>>> Sample: {nameof(UseHierarchicalPartitionKeys)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        Guid userSessionId;

        using (var context = new UserSessionContext())
        {
            #region Inserts
            var tenantId = "Microsoft";
            var sessionId = 7;

            context.AddRange(
                new UserSession
                {
                    TenantId = tenantId,
                    UserId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C"),
                    SessionId = sessionId,
                    Username = "mac"
                },
                new UserSession
                {
                    TenantId = tenantId,
                    UserId = new Guid("ADAE5DDE-8A67-432D-9DEC-FD7EC86FD9F6"),
                    SessionId = sessionId,
                    Username = "toast"
                },
                new UserSession
                {
                    TenantId = tenantId,
                    UserId = new Guid("61967254-AFF8-493A-B7F8-E62DA36D8367"),
                    SessionId = sessionId,
                    Username = "willow"
                },
                new UserSession
                {
                    TenantId = tenantId,
                    UserId = new Guid("BC0150CF-5147-44B8-8823-865F4F2323E1"),
                    SessionId = sessionId,
                    Username = "alice"
                });

            await context.SaveChangesAsync();
            #endregion

            userSessionId = context.ChangeTracker.Entries<UserSession>().Single(e => e.Entity.Username == "willow").Entity.Id;
        }

        Console.WriteLine();
        Console.WriteLine("Use Find to create a point-read:");
        Console.WriteLine();

        using (var context = new UserSessionContext())
        {
            #region FindAsync
            var tenantId = "Microsoft";
            var sessionId = 7;
            var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");

            var session = await context.Sessions.FindAsync(
                userSessionId, tenantId, userId, sessionId);
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Execute a query with full hierarchical partition key info:");
        Console.WriteLine();

        using (var context = new UserSessionContext())
        {
            #region FullPartitionKey
            var tenantId = "Microsoft";
            var sessionId = 7;
            var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");

            var sessions = await context.Sessions
                .Where(
                    e => e.TenantId == tenantId
                         && e.UserId == userId
                         && e.SessionId == sessionId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Execute a query with only top two levels of hierarchical partition key:");
        Console.WriteLine();

        using (var context = new UserSessionContext())
        {
            #region TopTwoPartitionKey
            var tenantId = "Microsoft";
            var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");

            var sessions = await context.Sessions
                .Where(
                    e => e.TenantId == tenantId
                         && e.UserId == userId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Execute a query with only top level of hierarchical partition key:");
        Console.WriteLine();

        using (var context = new UserSessionContext())
        {
            #region TopOnePartitionKey
            var tenantId = "Microsoft";

            var sessions = await context.Sessions
                .Where(
                    e => e.TenantId == tenantId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Execute a queries with incomplete subkey values:");
        Console.WriteLine();

        using (var context = new UserSessionContext())
        {
            var sessionId = 7;
            var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");

            #region BottomPartitionKey
            var sessions1 = await context.Sessions
                .Where(
                    e => e.SessionId == sessionId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion

            #region MiddlePartitionKey
            var sessions2 = await context.Sessions
                .Where(
                    e => e.UserId == userId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion

            #region BottomTwoPartitionKey
            var sessions3 = await context.Sessions
                .Where(
                    e => e.SessionId == sessionId
                         && e.UserId == userId
                         && e.Username.Contains("a"))
                .ToListAsync();
            #endregion
        }
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            await using var context = new UserSessionContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    #region UserSession
    public class UserSession
    {
        // Item ID
        public Guid Id { get; set; }

        // Partition Key
        public string TenantId { get; set; } = null!;
        public Guid UserId { get; set; }
        public int SessionId { get; set; }

        // Other members
        public string Username { get; set; } = null!;
    }
    #endregion

    public class UserSessionContext : DbContext
    {
        public DbSet<UserSession> Sessions { get; set; }

        private readonly bool _quiet;

        public UserSessionContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region HasPartitionKey
            modelBuilder
                .Entity<UserSession>()
                .HasPartitionKey(e => new { e.TenantId, e.UserId, e.SessionId });
            #endregion

            // See https://github.com/dotnet/efcore/issues/33961
            modelBuilder
                .Entity<UserSession>()
                .Property(e => e.Id).ValueGeneratedOnAdd();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "HierarchicalPartitionKeys",
                    cosmosOptionsBuilder =>
                    {
                        cosmosOptionsBuilder.HttpClientFactory(
                            () => new HttpClient(
                                new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback =
                                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                }));
                    });

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }
    }
}
