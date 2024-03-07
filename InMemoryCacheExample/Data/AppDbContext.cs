using InMemoryCacheExample.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCacheExample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<BlogDataModel> Blogs { get; set; }
        public DbSet<UserDataModel> Users { get; set; }
    }
}
