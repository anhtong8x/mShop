using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Fee { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string Provider { get; set; }

        public Guid UserId { get; set; }

        // 1 user co nhieu Transaction
        public AppUser AppUser { get; set; }
    }
}