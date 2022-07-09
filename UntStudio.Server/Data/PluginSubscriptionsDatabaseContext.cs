using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data;

public sealed class PluginSubscriptionsDatabaseContext : DbContext
{
    public PluginSubscriptionsDatabaseContext(DbContextOptions<PluginSubscriptionsDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
        PluginSubscription plugin = new PluginSubscription("PluginTestUnt", "1234-1234-1234-1234", "SOME IP");
        plugin.SetFree();
        Data.Add(plugin);
        SaveChanges();
    }



    public DbSet<PluginSubscription> Data { get; set; }
}
