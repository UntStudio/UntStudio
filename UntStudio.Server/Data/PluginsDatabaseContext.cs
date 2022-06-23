using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data
{
    public sealed class PluginsDatabaseContext : DbContext
    {
        public PluginsDatabaseContext(DbContextOptions<PluginsDatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
            /*Data.Add(new Plugin("RustSleeper45", "1234-1234-1234-1234", "58.1922.123.120:27015", DateTime.Now.AddDays(123)));
            SaveChanges();*/
        }



        public DbSet<Plugin> Data { get; set; }
    }
}
