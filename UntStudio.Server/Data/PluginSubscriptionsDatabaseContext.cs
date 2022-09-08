using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data;

public sealed class PluginSubscriptionsDatabaseContext : DbContext
{
    public PluginSubscriptionsDatabaseContext(DbContextOptions<PluginSubscriptionsDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }



    public DbSet<PluginSubscription> Data { get; set; }
}
