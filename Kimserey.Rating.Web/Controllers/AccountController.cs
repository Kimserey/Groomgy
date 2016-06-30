using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kimserey.Rating.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private AuthService _authService;
        private IUserService _userService;
        private IFileStorageService _fileStorageService;
        private IEmailService _emailService;

        public AccountController(IUserService userService, IFileStorageService fileStorageService, IEmailService emailService)
        {
            _authService = new AuthService();
            _userService = userService;
            _fileStorageService = fileStorageService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Confirm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            this.ValidateReturnUrl(returnUrl);
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return Redirect("/account");
            }

            var externalLogin = ExternalLoginData.FromIdentity(loginInfo.ExternalIdentity as ClaimsIdentity);
            returnUrl = string.Format("{0}?provider={1}&accessToken={2}", returnUrl, externalLogin.LoginProvider, externalLogin.AccessToken);
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExternalLoginGetToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken token = await this.ParseAccessToken(provider, accessToken);
            IdentityUser user = await _authService.FindUser(new UserLoginInfo(provider, token.UserIdProvider));
            if (user == null)
            {
                var result = await _authService.Create(new IdentityUser
                {
                    Email = token.Email,
                    UserName = token.Email
                });

                if (result.Errors.Any())
                {
                    return Json(new
                    {
                        error = result.Errors.First(),
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }

                Guid userId = await this.FindUserIdFromEmail(token.Email);
                await _authService.AddLoginExtenalLogin(userId.ToString(), new UserLoginInfo(provider, token.UserIdProvider));
                await _userService.CreateUser(userId, token.Email, token.UserName);

                string photoUrl;
                switch (provider)
                {
                    case "Facebook": photoUrl = await this.DownloadFacebookProfilePhoto(userId, token.UserIdProvider); break;
                    case "Google": photoUrl = await this.DownloadGoogleProfilePhoto(userId, accessToken); break;
                    default: throw new ApplicationException("Provider not supported");
                }

                await _userService.SetProfilePhotoUrl(userId, photoUrl);
                await _userService.UpdateGender(userId, token.Gender);
                user = await _authService.FindUser(new UserLoginInfo(provider, token.UserIdProvider));
            }

            //generate access token response
            var accessTokenResponse = TokenGenerator.GenerateAccessToken(user.Id, user.Email);
            return Json(new
            {
                token = new
                {
                    userName = accessTokenResponse.Value<string>("userName"),
                    access_token = accessTokenResponse.Value<string>("access_token"),
                    token_type = accessTokenResponse.Value<string>("token_type"),
                    expires_in = accessTokenResponse.Value<string>("expires_in")
                },
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return new EmptyResult();
        }

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            IdentityUser user;
            using (var authDb = new AuthDbContext())
            {
                user = await authDb.Users.SingleOrDefaultAsync(u => u.Email == userModel.Email);
            }

            if (user != null
                && !await _authService.IsEmailConfirmed(new Guid(user.Id)))
            {
                return Json(new { success = false, canResendConfirmation = true, errors = new[] { "This email is used but has not been verified." } });
            }

            IdentityResult result = await _authService.RegisterUser(userModel);

            if (result.Succeeded)
            {
                await SendVerificationEmail(userModel.Email);
                return Json(new { success = true });
            }

            return Json(new { success = false, canResendConfirmation = false, errors = result.Errors });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SendConfirmEmail(string email)
        {
            await this.SendVerificationEmail(email);
            return Json(new { success = true });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ConfirmEmail(string email, string token)
        {
            Guid userId = await this.FindUserIdFromEmail(email);
            var result = await _authService.ConfirmEmail(userId, token);
            await _userService.CreateUser(userId, email);

            return Json(new { success = result.Succeeded });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SendResetPasswordEmail(string email)
        {
            Guid userId = await this.FindUserIdFromEmail(email);
            string token = await _authService.GeneratePasswordResetToken(userId);
            await _emailService.SendResetPasswordEmail(email, token);

            return Json(new { success = true });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            Guid userId = await this.FindUserIdFromEmail(model.Email);
            var identityResult = await _authService.ResetPassword(userId, model.Token, model.Password);

            return Json(new { success = identityResult.Succeeded });
        }

        private Task<string> DownloadFacebookProfilePhoto(Guid userId, string userIdProvider)
        {
            using (WebClient client = new WebClient())
            {
                Uri uri = new Uri(string.Format(@"https://graph.facebook.com/{0}/picture?width=1280", userIdProvider));
                var photoData = client.DownloadData(uri);
                return _fileStorageService.SaveFile(userId, "facebook_profile_image", ".JPG", new MemoryStream(photoData));
            }
        }

        private Task<string> DownloadGoogleProfilePhoto(Guid userId, string accessToken)
        {
            using (WebClient client = new WebClient())
            {
                Uri uri = new Uri(string.Format("https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}", accessToken));
                var data = JsonConvert.DeserializeObject<GoogleUserData>(client.DownloadString(uri));
                uri = new Uri(data.Picture);
                var photoData = client.DownloadData(uri);
                return _fileStorageService.SaveFile(userId, "google_profile_image", ".JPG", new MemoryStream(photoData));
            }
        }

        private async Task SendVerificationEmail(string email)
        {
            Guid userId = await this.FindUserIdFromEmail(email);
            var token = await _authService.GenerateVerificationCode(userId);
            await _emailService.SendConfirmationEmail(email, token);
        }

        private async Task<Guid> FindUserIdFromEmail(string email)
        {
            Guid userId;
            using (var authDb = new AuthDbContext())
            {
                userId = new Guid((await authDb.Users.SingleAsync(u => u.Email == email)).Id);
            }
            return userId;
        }

        private void ValidateReturnUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Relative, out uri))
            {
                throw new UnauthorizedAccessException();
            }
        }

        private async Task<ParsedExternalAccessToken> ParseAccessToken(string provider, string accessToken)
        {
            await this.VerifyExternalAccessToken(provider, accessToken);

            string json;
            string url = "";

            if (provider == "Facebook")
            {
                url = string.Format("https://graph.facebook.com/me?access_token={0}", accessToken);
            }
            else if (provider == "Google")
            {
                url = string.Format("https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}", accessToken);
            }
            else
            {
                throw new ApplicationException("Provider not supported.");
            }

            using (var client = new HttpClient())
            {
                json = await client.GetStringAsync(url);
            }
            dynamic jsonObject = JsonConvert.DeserializeObject(json);
            var parsedToken = new ParsedExternalAccessToken
            {
                Email = jsonObject.email,
                UserName = jsonObject.name,
                UserIdProvider = jsonObject.id
            };

            if (jsonObject.gender == "male")
            {
                parsedToken.Gender = Gender.Man;
            }
            else if (jsonObject.gender == "female")
            {
                parsedToken.Gender = Gender.Woman;
            }
            else
            {
                parsedToken.Gender = Gender.NotSpecified;
            }

            return parsedToken;
        }

        private async Task VerifyExternalAccessToken(string provider, string accessToken)
        {
            var url = "";
            if (provider == "Facebook")
            {
                url = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}",
                    accessToken,
                    Consts.FacebookApp.AccessTokenAppId);
            }
            else if (provider == "Google")
            {
                url = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                throw new ApplicationException("Provider not supported");
            }

            using (var client = new HttpClient())
            {
                dynamic json = JsonConvert.DeserializeObject(await client.GetStringAsync(url));
                var verification = new AccessTokenVerification();

                if (provider == "Facebook")
                {
                    verification.UserId = json["data"]["user_id"];
                    verification.AppId = json["data"]["app_id"];

                    if (!string.Equals(Startup.FacebookAuthOptions.AppId, verification.AppId, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ApplicationException("Invalid access token");
                    }
                }
                else if (provider == "Google")
                {
                    verification.UserId = json["user_id"];
                    verification.AppId = json["audience"];

                    if (!string.Equals(Startup.GoogleAuthOptions.ClientId, verification.AppId, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ApplicationException("Invalid access token");
                    }

                }
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}