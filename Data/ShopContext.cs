using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodKept.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FoodKept.Data
{
    public class ShopContext : IdentityDbContext
    {
        public ShopContext (DbContextOptions<ShopContext> options)
            : base(options)
        {
        }

        public DbSet<FoodKept.Models.Food> FoodData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Food>().ToTable("Food");
        }
    }
}
