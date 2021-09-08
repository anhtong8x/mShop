using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;

namespace mShop.Data.Configurations
{
    internal class AppConfigConfiguration : IEntityTypeConfiguration<AppConfig>
    {
        public void Configure(EntityTypeBuilder<AppConfig> builder)
        {
            builder.ToTable("AppConfigs");  // Ten bang
            builder.HasKey(x => x.Key);     // Khoa chinh
            builder.Property(x => x.Value).IsRequired();    // default is true
        }
    }
}