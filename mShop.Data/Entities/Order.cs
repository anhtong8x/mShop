using mShop.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipEmail { get; set; }
        public string ShipPhoneNumber { get; set; }
        public OrderStatus Status { get; set; }

        // 1 user co nhieu order
        public AppUser AppUser { get; set; }

        // 1 order co nhieu ordertal
        public List<OrderDetail> OrderDetails { get; set; }
    }
}