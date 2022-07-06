using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

builder.Services.AddControllers();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
