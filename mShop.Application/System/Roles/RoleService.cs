using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mShop.Data.Entities;
using mShop.ViewModel.System.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mShop.Application.System.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> mRoleManager;

        public RoleService(RoleManager<AppRole> nRoleManager)
        {
            mRoleManager = nRoleManager;
        }
        public async Task<List<RoleViewModel>> GetAll()
        {
            // lay ve tat ca cac role
            var roles = await mRoleManager.Roles
                .Select(x => new RoleViewModel() {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();
            return roles;
        }
    }
}
