using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UserManager : IUserService
    {
        IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUsername(username);
        }

        public async Task<UserManagerResponse> UpdateUserPassword(ChangePasswordViewModel model)
        {
            return await _userRepository.UpdateUserPassword(model);
        }

        public async Task<UserManagerResponse> UpdateUserProfile(User user)
        {
            return await _userRepository.UpdateUserProfile(user);
        }
    }
}
