using Microsoft.AspNetCore.Identity;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IRoleRepository
    {
        Task<List<IdentityRole>> GetAllRoles();

        Task<UserManagerResponse> AddUserToRole(string email, string roleName);

        Task<UserManagerResponse> CreateRole(string roleName);

        Task<IList<string>> GetUserRoles(string email);

        Task<UserManagerResponse> RemoveUserFromRole(string email, string roleName);
    }
}
