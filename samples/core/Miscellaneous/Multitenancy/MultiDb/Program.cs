using Common;
using Microsoft.EntityFrameworkCore;
using MultiDb;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddDbContextFactory<ContactContext>(opt => { }, ServiceLifetime.Scoped);

WebApplication app = builder.Build();

// just for demo to avoid migrations, don't do this in production
IServiceProvider provider = app.Services.CreateScope().ServiceProvider;
ITenantService? tenantService = provider.GetService<ITenantService>();
IDbContextFactory<ContactContext>? factory = provider.GetService<IDbContextFactory<ContactContext>>();
if (tenantService != null && factory != null)
{
    foreach (var tenant in tenantService.GetTenants())
    {
        tenantService.SetTenant(tenant);
        using ContactContext ctx = factory.CreateDbContext();
        ctx.CheckAndSeed();
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
