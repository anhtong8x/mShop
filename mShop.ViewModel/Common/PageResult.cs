﻿using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.Common
{
    public class PageResult<T>
    {
        public int TotalPage { get; set; }
        public List<T> Items { get; set; }
    }
}