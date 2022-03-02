using System;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.ModelBuilding;

public class OptionsContext : DbContext
{
    #region Configuration
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseCosmos(
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            databaseName: "OptionsDB",
            options =>
            {
                options.ConnectionMode(ConnectionMode.Gateway);
                options.WebProxy(new WebProxy());
                options.LimitToEndpoint();
                options.Region(Regions.AustraliaCentral);
                options.GatewayModeMaxConnectionLimit(32);
                options.MaxRequestsPerTcpConnection(8);
                options.MaxTcpConnectionsPerEndpoint(16);
                options.IdleTcpConnectionTimeout(TimeSpan.FromMinutes(1));
                options.OpenTcpConnectionTimeout(TimeSpan.FromMinutes(1));
                options.RequestTimeout(TimeSpan.FromMinutes(1));
            });
    #endregion
}