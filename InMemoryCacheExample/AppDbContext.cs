using InMemoryCacheExample.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCacheExample
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<BlogDataModel> Blogs { get; set; }
    }
}
