using System;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.ModelBuilding
{
    public class OptionsContext : DbContext
    {
        #region Configuration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseCosmos(
                new System.Text.RegularExpressions.Regex("\\\\").Replace(Environment.GetEnvironmentVariable("COSMOS_ENDPOINT"), "/"),
                Environment.GetEnvironmentVariable("COSMOS_ACCOUNTKEY"),
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
}
