using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;

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
builder.Services.AddHttpClient(KnownHttpClientNames.AdminsAPI, configure =>
{
    //configure.BaseAddress = new Uri("https://localhost:7192/admin/");
    configure.BaseAddress = new Uri("https://untstudioserver20220710162140.azurewebsites.net/admin/");
    configure.DefaultRequestHeaders.Add(HeaderNames.UserAgent, KnownHeaders.UserAgentAdminValue);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(configure =>
    {
        configure.LoginPath = "/login";
        configure.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment() == false)
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
