using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;
using System;

namespace mShop.Data.Configurations
{
    public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            builder.ToTable("ProductTranslations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SeoAlias).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Details).HasMaxLength(500);
            builder.Property(x => x.LanguageId).IsUnicode(false).IsRequired().HasMaxLength(5);

            // Bang product va language quan he nhieu nhieu. Ta tao 1 bang trung gian producttranslation
            // 2 bang se quan he 1 nhieu voi bang trung dan
            // 1 language nhieu producttranslation
            builder.HasOne(x => x.Language).WithMany(x => x.ProductTranslations).HasForeignKey(x => x.LanguageId);

            //1 product nhieu producttranslation
            builder.HasOne(x => x.Product).WithMany(x => x.ProductTranslations).HasForeignKey(x => x.ProductId);
        }
    }
}