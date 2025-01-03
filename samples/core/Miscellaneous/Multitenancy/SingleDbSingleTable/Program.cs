using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using SingleDbSingleTable.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddDbContextFactory<ContactContext>(
    opt => opt.UseSqlite("Data Source=singledb.sqlite"), ServiceLifetime.Scoped);

var app = builder.Build();

// seed the database so demo is simple and doesn't require migrations
using var ctx = app.Services.CreateScope().ServiceProvider.GetRequiredService<ContactContext>();
await ctx.CheckAndSeedAsync();

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
