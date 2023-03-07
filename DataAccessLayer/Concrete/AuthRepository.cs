using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataAccessLayer.Concrete
{
    public class AuthRepository : IAuthRepository
    {
        private UserManager<IdentityUser> _userManager;
        private ApplicationDbContext db;
        private IConfiguration _configuration;
        private IMailRepository _mailRepository;

        public AuthRepository(UserManager<IdentityUser> userManager, ApplicationDbContext db, IConfiguration configuration, IMailRepository mailRepository)
        {
            _userManager = userManager;
            this.db = db;
            _configuration = configuration;
            _mailRepository = mailRepository;
        }

        public async Task<UserManagerResponse> LoginUser(LoginViewModel model)
        {
            if (model == null)
                throw new NullReferenceException("Login model is null!");

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Girdiğiniz email adresine ait hesap bulunamadı!",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Email veya şifreniz hatalı!",
                    IsSuccess = false,
                };
            }

            var emailConfirmExist = user.EmailConfirmed;
            if (!emailConfirmExist)
            {
                return new UserManagerResponse
                {
                    Message = "Giriş yapmak için email adresinizi doğrulamanız gerekmektedir!",
                    IsSuccess = false,
                };
            }

            var jwtToken = await GenerateJwtToken(user);

            var authResult = new AuthResult
            {
                Token = jwtToken.Token,
                Success = jwtToken.Success,
                Errors = jwtToken.Errors
            };

            return new UserManagerResponse
            {
                Message = "",
                IsSuccess = true,
                AuthResult = authResult
            };
        }

        public async Task<UserManagerResponse> RegisterUser(RegisterViewModel model)
        {
            if (model == null)
                throw new NullReferenceException("Register model is null!");

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            var existingUsername = await _userManager.FindByNameAsync(model.Username);

            if (existingEmail != null)
            {
                return new UserManagerResponse
                {
                    Message = "Bu email zaten kullanılıyor!",
                    IsSuccess = false
                };
            }

            if (existingUsername != null)
            {
                return new UserManagerResponse
                {
                    Message = "Bu kullanıcı adı zaten kullanılıyor!",
                    IsSuccess = false
                };
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Şifreniz onay şifresi ile eşleşmiyor!",
                    IsSuccess = false
                };
            }

            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username
            };

            var userDetails = new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Username = model.Username,
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppURL"]}/api/auth/confirmemail?userid={user.Id}&token={validEmailToken}";

                string subject = "E postanızı onaylayınız";
                string body = $"<h1>Rehber App</h1>" +
                    $"<p>E postanızı onaylamak için lütfen <a href='{url}'>Tıklayınız</a></p>";

                await _mailRepository.SendEmailAsync(user.Email, subject, body);

                db.Users.Add(userDetails);
                await db.SaveChangesAsync();

                return new UserManagerResponse
                {
                    Message = "Kullanıcı başarıyla oluşturuldu",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "Kullanıcı oluşturulamadı!",
                IsSuccess = false,
                Errors = result.Errors.Select(x => x.Description)
            };
        }

        public async Task<UserManagerResponse> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Girdiğiniz email adresine ait kullanıcı bulunamadı!",
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["ClientURL"]}/auth/reset_password?email={email}&token={validToken}";

            string subject = "Şifre Yenileme";
            string body = $"<h1>RehberApp</h1>" + 
                $"<p>Şifrenizi sıfırlamak için <a href='{url}'>Tıklayınız</a></p>";

            await _mailRepository.SendEmailAsync(email, subject, body);

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Şifre sıfırlama email adresinize gönderildi."
            };
        }

        public async Task<UserManagerResponse> ResetPassword(ResetPasswordViewModel model)
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

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Şifreniz onay şifresi ile eşleşmiyor!"
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Şifreniz başarıyla değiştirildi!"
                };
            }

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Hata oluştu!",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new AuthResult
            {
                Token = jwtToken,
                Success = true,
            };
        }

        public async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var userDetails = await db.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDetails.USER_ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, userDetails.Name),
                new Claim(ClaimTypes.Surname, userDetails.Surname),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }

        public async Task<UserManagerResponse> ConfirmEmail(string userid, string token)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Kullanıcı bulunamadı!",
                    IsSuccess = false
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if(result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email başarıyla onaylandı.",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email onaylanmadı!",
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}
