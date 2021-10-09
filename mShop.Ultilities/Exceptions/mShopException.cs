using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.Ultilities.Exceptions
{
    public class mShopException : Exception
    {
        public mShopException()
        {
        }

        public mShopException(string message) : base(message)
        {
        }

        public mShopException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}