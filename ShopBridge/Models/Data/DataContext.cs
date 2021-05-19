using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using ShopBridge.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ShopBridge.Data
{
    
    public class DataContext:DbContext
    {
        public DataContext() : base() { }
        public DataContext(DbContextOptions opt) : base() { 
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("ShopBridgeContext");

            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Beverage" },
                new Category { CategoryId = 2, CategoryName = "Laundry", Description = "Laundry Products" },
                new Category { CategoryId = 3, CategoryName = "Oil", Description = "Edible Oil" }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Olive Oil", Description = " Olive Oil", CategoryId = 3 },
                new Product { Id = 2, Name = "Sunflower Oil", Description = " Sunflower Oil", CategoryId = 3 },
                new Product { Id = 3, Name = "RiceBran Oil", Description = " RiceBran Oil", CategoryId = 3 },
                new Product { Id = 4, Name = "Pepsi", Description = " Pepsi", CategoryId = 1 },
                new Product { Id = 5, Name = "Bovanto", Description = " Bovanto", CategoryId = 1 },
                new Product { Id = 6, Name = "Coke", Description = " Coke", CategoryId = 1 },
                new Product { Id = 7, Name = "Surf Excel", Description = " Surf Excel", CategoryId = 2 },
                new Product { Id = 8, Name = "Arial", Description = " Arial", CategoryId = 2 },
                new Product { Id = 9, Name = "Comfort", Description = " Comfort", CategoryId = 2 }

            );

           

        }

    }


}
