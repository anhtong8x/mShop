using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class Product
    {
        public int Id { set; get; }
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public int ViewCount { set; get; }
        public DateTime DateCreated { set; get; }

        // 1 categori co nhieu product. Quan he 1( Categories ) nhieu ( Product )
        public List<ProductInCategory> ProductInCategories { get; set; }

        // 1 order co nhieu OrderDetail
        public List<OrderDetail> OrderDetails { get; set; }

        // 1 order co nhieu Carts
        public List<Cart> Carts { get; set; } // 1 cart co nhieu product

        // 1 product co nhieu producttranslation
        public List<ProductTranslation> ProductTranslations { get; set; }

        // 1 product co nhieu productimage
        public List<ProductImage> ProductImages { get; set; }
    }
}