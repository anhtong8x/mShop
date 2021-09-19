using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Application.Catalog.Products.Dtos
{
    public class ProductCreateRequest
    {
        public String Name { get; set; }

        public decimal Price { get; set; }
    }
}