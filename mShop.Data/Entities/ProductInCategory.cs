using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class ProductInCategory
    {
        // bang trung gian 2 bang
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}