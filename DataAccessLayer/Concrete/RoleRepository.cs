using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class RoleRepository : IRoleRepository
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext db;

        public RoleRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.db = db;
        }

        public async Task<List<IdentityRole>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<UserManagerResponse> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Kullanıcı bulunamadı!",
                    IsSuccess = false
                };
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                return new UserManagerResponse
                {
                    Message = "Rol bulunamadı!",
                    IsSuccess = false
                };
            }

            var currentRole = await GetUserRoles(email);

            if (currentRole.Count > 0)
            {
                var deleteCurrentRole = await RemoveUserFromRole(email, currentRole[0]);
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            var userDetails = db.Users.FirstOrDefault(x => x.Email == email);

            if (userDetails == null)
            {
                return new UserManagerResponse
                {
                    Message = "Kullanıcı bulunamadı!",
                    IsSuccess = false
                };
            }

            userDetails.Role = roleName;
            db.Users.Update(userDetails);
            await db.SaveChangesAsync();

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = $"{email} Mail adresine ait kullanıcı {currentRole[0]} rolünden kaldırıldı, {roleName} rolüne eklendi.",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = "Kullanıcı role eklenemedi!",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = $"{roleName} adlı rol oluşturuldu",
                        IsSuccess = true
                    };
                }

                return new UserManagerResponse
                {
                    Message = "Rol oluşturulamadı!",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserManagerResponse
            {
                Message = "Bu rol zaten eklenmiş!",
                IsSuccess = false,
            };
        }

        public async Task<IList<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);

            return roles;
        }

        public async Task<UserManagerResponse> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Kullanıcı bulunamadı!",
                    IsSuccess = false
                };
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                return new UserManagerResponse
                {
                    Message = "Rol bulunamadı!",
                    IsSuccess = false
                };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Rol kaldırıldı!",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = "Rol kaldırılamadı!",
                IsSuccess = false
            };
        }
    }
}
