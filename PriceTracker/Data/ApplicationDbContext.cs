using System.Numerics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Models;

namespace PriceTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //// we override the OnModelCreating method here.
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ProductCategory>().HasKey(vf => new { vf.ProductId, vf.CategoryId });
        //}

        public DbSet<Category> Categories { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<PriceEntry> PriceEntries { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }


    }
}
