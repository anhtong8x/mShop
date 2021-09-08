using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.ToTable("ProductInCategories");
            builder.HasKey(t => new { t.CategoryId, t.ProductId });

            // 1 product co nhieu ProductInCategories. Khoa ngoai ProductId
            builder.HasOne(t => t.Product).WithMany(pc => pc.ProductInCategories)
                .HasForeignKey(pc => pc.ProductId);

            // 1 Category co nhieu ProductInCategories. Khoa ngoai CategoryId
            builder.HasOne(t => t.Category).WithMany(pc => pc.ProductInCategories)
              .HasForeignKey(pc => pc.CategoryId);
        }
    }
}