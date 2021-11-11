using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.Common
{
    public class ApiErrorResult<T> : ApiResult<T>
    {
        public string[] ValidationErrors { get; set; }

        public ApiErrorResult()
        {
        }

        // khi tra ve 1 message
        public ApiErrorResult(string message)
        {
            IsSuccessed = false;
            Message = message;
        }

        // khi tra ve 1 mang cac message
        public ApiErrorResult(string[] validationErrors)
        {
            IsSuccessed = false;
            ValidationErrors = validationErrors;
        }
    }
}