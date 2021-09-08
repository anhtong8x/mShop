using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public bool ApplyForAll { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string ProductIds { get; set; }
        public string ProductCategoryids { get; set; }
        public Status Status { get; set; }
        public string Name { get; set; }
    }
}