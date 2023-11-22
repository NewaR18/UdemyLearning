using AspNetCoreFromBasic.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreFromBasic.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Library> Library { get; set; }
    }
}
