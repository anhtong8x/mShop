using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mShop.Data.Entities;

namespace mShop.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");
            builder.HasKey(x => x.Id);  // khoa chinh
            builder.Property(x => x.Id).UseIdentityColumn(); // kieu iden cho khoa chinh

            // Khai bao khoa ta chi tham chieu cac thuoc tinh trong class Cart
            // Cac class lien quan ta khai bao cac thuoc tinh tham chieu den
            // 1 cart co nhieu product
            // class product khai bao 1 bien product
            // class cart khai bao 1 list product

            builder.HasOne(x => x.Product).WithMany(x => x.Carts).HasForeignKey(x => x.ProductId);

            // 1 user co nhieu gio hang
            // ta khai bao chi tham chieu cac cot trong bang cart
            builder.HasOne(x => x.AppUser).WithMany(x => x.Carts).HasForeignKey(x => x.UserId);
        }
    }
}