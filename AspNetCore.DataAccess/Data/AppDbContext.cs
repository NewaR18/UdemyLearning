using AspNetCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Library> Library { get; set; }
        public DbSet<CoverType> CoverType { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<PaymentKhalti> PaymentKhalti { get; set; }
        public DbSet<EsewaPayment> EsewaPayment { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
    }
}
