using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IAuthRepository
    {
        Task<UserManagerResponse> LoginUser(LoginViewModel model);

        Task<UserManagerResponse> RegisterUser(RegisterViewModel model);

        Task<UserManagerResponse> ConfirmEmail(string userid, string token);

        Task<UserManagerResponse> ForgotPassword(string email);

        Task<UserManagerResponse> ResetPassword(ResetPasswordViewModel model);
    }
}
