using Microsoft.EntityFrameworkCore;
using FurnitureMarketplace.Models;

namespace FurnitureMarketplace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FurnitureItem> FurnitureItems { get; set; }
        public DbSet<ServiceItem> Services { get; set; }
        public DbSet<User> Users { get; set; }
    }
}