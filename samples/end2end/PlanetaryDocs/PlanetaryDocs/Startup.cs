using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlanetaryDocs.DataAccess;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs
{
    /// <summary>
    /// Blazor startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration information.</param>
        public Startup(IConfiguration configuration) => Configuration = configuration;

        /// <summary>
        /// Gets t configuration instance.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Select the services used by your app.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.Configure<CosmosSettings>(
                Configuration.GetSection(nameof(CosmosSettings)));
            services.AddDbContextFactory<DocsContext>(
               (IServiceProvider sp, DbContextOptionsBuilder opts) =>
               {
                   var cosmosSettings = sp
                       .GetRequiredService<IOptions<CosmosSettings>>()
                       .Value;

                   opts.UseCosmos(
                       cosmosSettings.EndPoint,
                       cosmosSettings.AccessKey,
                       nameof(DocsContext));
               });

            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<LoadingService>();
            services.AddScoped<HistoryService>();
            services.AddScoped<TitleService>();
            services.AddScoped<MultiLineEditService>();
        }

        /// <summary>
        /// Configure the selected services.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The current environment.</param>
        /// <param name="factory">Context factory.</param>
        /// <param name="cs">The Cosmos settings.</param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IDbContextFactory<DocsContext> factory,
            IOptions<CosmosSettings> cs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (cs.Value.EnableMigration)
            {
                using var context = factory.CreateDbContext();
                context.CheckAndMigrateTagsAsync(cs.Value.DocumentToCheck).
                    ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
