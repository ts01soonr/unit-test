using EntityFrameworkCore.MySQL.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MySQL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
