using AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.DataAccess.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Library> Library { get; set; }
        public DbSet<CoverType> CoverType { get; set; }

    }
}
