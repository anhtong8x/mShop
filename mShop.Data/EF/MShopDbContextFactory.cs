using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mShop.Data.EF
{
    public class mShopDbContextFactory : IDesignTimeDbContextFactory<MShopDbContext>
    {
        public MShopDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("")
                .Build();

            // chuoi conection string lay tu file appsetting.json

            var connectionString = configuration.GetConnectionString("mShopDb");

            var optionsBuilder = new DbContextOptionsBuilder<MShopDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MShopDbContext(optionsBuilder.Options);
        }
    }
}