using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace mShop.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }

        // 1 user co nhieu gio hang
        public List<Cart> Carts { get; set; }

        // 1 user co nhieu order
        public List<Order> Orders { get; set; }

        // 1 user co nhieu transaction
        public List<Transaction> Transactions { get; set; }
    }
}