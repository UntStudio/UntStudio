using Microsoft.EntityFrameworkCore;
using UntStudio.Server.Models;

namespace UntStudio.Server.Data;

public sealed class AdminsDatabaseContext : DbContext
{
    public AdminsDatabaseContext(DbContextOptions<AdminsDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }



    public DbSet<Admin> Data { get; set; }
}
