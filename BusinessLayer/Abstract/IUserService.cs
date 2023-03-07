using EntityLayer.Concrete;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IUserService
    {
        Task<User> GetUserByUsername(string username);

        Task<UserManagerResponse> UpdateUserProfile(User user);

        Task<UserManagerResponse> UpdateUserPassword(ChangePasswordViewModel model);
    }
}
