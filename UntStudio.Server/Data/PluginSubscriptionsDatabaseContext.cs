using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data;

public sealed class PluginSubscriptionsDatabaseContext : DbContext
{
    public PluginSubscriptionsDatabaseContext(DbContextOptions<PluginSubscriptionsDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
        PluginSubscription plugin = new PluginSubscription("PluginTestUnt", "1234-1234-1234-1234", "89.235.209.143");
        Data.Add(plugin);
        SaveChanges();
        //PluginSubscription plugin = new PluginSubscription("PluginTestUnt", "1234-1234-1234-1234", "SOME IP");
        //plugin.SetFree();
    }



    public DbSet<PluginSubscription> Data { get; set; }
}
