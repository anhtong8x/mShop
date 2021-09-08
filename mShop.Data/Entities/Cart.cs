using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }

        public int ProductId { get; set; } // khoa ngoai
        public Product Product { get; set; } // 1 cart co nhieu product

        public Guid UserId { get; set; }   // khoa ngoai
        public AppUser AppUser { get; set; }// 1 user co nhieu cart
    }
}