using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.OrderDate).HasDefaultValue(DateTime.Now);
            builder.Property(x => x.ShipEmail).IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.ShipAddress).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ShipName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ShipPhoneNumber).IsRequired().HasMaxLength(200);

            // 1 user co nhieu order
            builder.HasOne(x => x.AppUser).WithMany(x => x.Orders).HasForeignKey(x => x.UserId);
        }
    }
}