using AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.DataAccess
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Library> Library { get; set; }
    }
}
