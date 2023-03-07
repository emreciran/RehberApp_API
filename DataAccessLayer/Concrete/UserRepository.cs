using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class UserRepository : IUserRepository
    {
        private UserManager<IdentityUser> _userManager;
        private ApplicationDbContext db;

        public UserRepository(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            this.db = db;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var errors = new List<string>();

            if (user == null)
            {
                errors.Add("Kullanıcı bulunamadı!");
                return null;
            }

            var userDetails = await db.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (userDetails == null)
            {
                errors.Add("Kullanıcı bulunamadı!");
                return null;
            }

            return new User
            {
                CreatedDate = userDetails.CreatedDate,
                Email = userDetails.Email,
                Name = userDetails.Name,
                Surname = userDetails.Surname,
                Username = userDetails.Username,
                USER_ID = userDetails.USER_ID
            };
        }

        public async Task<UserManagerResponse> UpdateUserProfile(User user)
        {
            if (user == null)
            {
                return null;
            } 

            var userDetails = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.USER_ID == user.USER_ID);
            if (userDetails == null)
            {
                return null;
            }
            

            var updatedUser = new User
            {
                CreatedDate = userDetails.CreatedDate,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                USER_ID = userDetails.USER_ID
            };

            //var identityUser = await _userManager.FindByEmailAsync(userDetails.Email);
            //if (identityUser == null)
            //{
            //    return null;
            //}

            db.Users.Update(updatedUser);
            await db.SaveChangesAsync();

            return new UserManagerResponse
            {
                Message = "Kullanıcı bilgileri güncellendi!",
                IsSuccess = true
            };
        }

        public async Task<UserManagerResponse> UpdateUserPassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı!"
                };
            }

            if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Mevcut şifreniz hatalı!"
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Onay şifreniz hatalı!"
                };
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Yeni şifreniz eski şifrenizden farklı olmalıdır!"
                };
            }

            await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Şifreniz başarıyla değiştirildi!"
            };
        }
    }
}
