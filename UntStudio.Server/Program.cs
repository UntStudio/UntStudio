using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using UntStudio.Server.Bits;
using UntStudio.Server.Data;
using UntStudio.Server.Encryptors;
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


builder.Services.AddSingleton<IEncryptor, Encryptor>();
builder.Services.AddSingleton<IPEBit, PEBit>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(KnownHttpClientNames.AdminsAPI, configure =>
{
    configure.BaseAddress = new Uri("http://135.181.47.150/admin/");
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
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
