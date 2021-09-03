using Microsoft.EntityFrameworkCore;
using mShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.EF
{
    public class MShopDbContext : DbContext
    {
        // contructor
        public MShopDbContext(DbContextOptions options) : base(options)
        {
        }

        // khai bao cac class table
        public DbSet<Product> Products { get; set; }
    }
}