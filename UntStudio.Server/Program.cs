using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using UntStudio.Server.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PluginSubscriptionsDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PluginSubscriptionsDatabaseConnectionString"));
});

builder.Services.AddDbContext<AdminsDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdminsDatabaseConnectionString"));
});

builder.Services.AddLogging(configure =>
{
    configure.AddFile(builder.Configuration.GetSection("Logging"))
        .AddConsole();
});

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(configure =>
    {
        configure.LoginPath = "/login";
        configure.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
