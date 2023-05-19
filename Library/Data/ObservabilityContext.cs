using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class ObservabilityContext : DbContext
    {
        public ObservabilityContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Supply> Supplies { get; set; }
    }
}