using mShop.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.System.Users
{
    public class RoleAssignRequest
    {
        public Guid Id { get; set; }
        public List<SelectItem> Roles { get; set; } = new List<SelectItem>(); // gan gia tri mac dinh de no k bi null 

    }
}
