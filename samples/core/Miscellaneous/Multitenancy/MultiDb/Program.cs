using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MultiDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddDbContextFactory<ContactContext>(opt => { }, ServiceLifetime.Scoped);

var app = builder.Build();

// just for demo to avoid migrations, don't do this in production
var provider = app.Services.CreateScope().ServiceProvider;
var tenantService = provider.GetService<ITenantService>();
var factory = provider.GetService<IDbContextFactory<ContactContext>>();
if (tenantService != null && factory != null)
{
    foreach (var tenant in tenantService.GetTenants())
    {
        tenantService.SetTenant(tenant);
        using var ctx = factory.CreateDbContext();
        await ctx.CheckAndSeed();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
