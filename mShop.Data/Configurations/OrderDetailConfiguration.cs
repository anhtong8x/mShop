using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(x => new { x.OrderId, x.ProductId });    // khoa chinh la 2 cot

            // 1 order co nhieu orderdetail
            builder.HasOne(n => n.Order).WithMany(n => n.OrderDetails).HasForeignKey(n => n.OrderId);
            // 1 product co nhieu orderdetail
            builder.HasOne(n => n.Product).WithMany(n => n.OrderDetails).HasForeignKey(n => n.ProductId);
        }
    }
}