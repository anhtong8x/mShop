using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Application.Dtos
{
    public class PageResult<T>
    {
        public int TotlaPage { get; set; }
        public List<T> Items { get; set; }
    }
}