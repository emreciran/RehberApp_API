using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class AuthManager : IAuthService
    {
        IAuthRepository _userRepository;

        public AuthManager(IAuthRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserManagerResponse> ConfirmEmail(string userid, string token)
        {
            return await _userRepository.ConfirmEmail(userid, token);
        }

        public async Task<UserManagerResponse> ForgotPassword(string email)
        {
            return await _userRepository.ForgotPassword(email);
        }

        public async Task<UserManagerResponse> LoginUser(LoginViewModel model)
        {
            return await _userRepository.LoginUser(model);
        }

        public async Task<UserManagerResponse> RegisterUser(RegisterViewModel model)
        {
            return await _userRepository.RegisterUser(model);
        }

        public async Task<UserManagerResponse> ResetPassword(ResetPasswordViewModel model)
        {
            return await _userRepository.ResetPassword(model);
        }
    }
}
