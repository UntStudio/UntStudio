using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data;

public sealed class PluginsDatabaseContext : DbContext
{
    public PluginsDatabaseContext(DbContextOptions<PluginsDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }



    public DbSet<Plugin> Data { get; set; }
}
