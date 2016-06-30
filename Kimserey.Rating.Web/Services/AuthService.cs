using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrill;

namespace Kimserey.Rating.Web.Services
{
    public class AuthService : IDisposable
    {
        private AuthDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public AuthService()
        {
            _context = new AuthDbContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));

            var userTokenProvider = new DataProtectorTokenProvider<IdentityUser>(
                Startup.DataProtectionProvider.Create("EmailConfirmation"));
            _userManager.UserTokenProvider = userTokenProvider;
        }

        public async Task<IdentityResult> Create(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return result;
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.Email,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);
            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public async Task<IdentityResult> AddLoginExtenalLogin(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);
            return result;
        }

        public async Task<IdentityUser> FindUser(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<string> GenerateVerificationCode(Guid userId)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userId.ToString());
            return token;
        }

        public Task<IdentityResult> ConfirmEmail(Guid userId, string token)
        {
            return _userManager.ConfirmEmailAsync(userId.ToString(), token);
        }

        public Task<bool> IsEmailConfirmed(Guid userId)
        {
            return _userManager.IsEmailConfirmedAsync(userId.ToString());
        }

        public Task<string> GeneratePasswordResetToken(Guid userId)
        {
            return _userManager.GeneratePasswordResetTokenAsync(userId.ToString());
        }

        public Task<IdentityResult> ResetPassword(Guid userId, string token, string newPassword)
        {
            return _userManager.ResetPasswordAsync(userId.ToString(), token, newPassword);
        }

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
        }
    }

}
