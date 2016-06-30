using Kimserey.Rating.Web.Dto;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Kimserey.Rating.Web.Persistence.Entities;
using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kimserey.Rating.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ProfileController : Controller
    {
        private IUserService _userService;
        private IFileStorageService _fileStorageService;
        private IHubContext<IProfileHubClient> _hubContext;
        private IOnlineUserService _onlineUserService;

        public ProfileController(IUserService userService,
            IOnlineUserService onlineUserService,
            IFileStorageService fileStorageService,
            IHubContext<IProfileHubClient> hubContext)
        {
            _userService = userService;
            _onlineUserService = onlineUserService;
            _fileStorageService = fileStorageService;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserProfile()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult DeleteFromGallery()
        {
            return View();
        }

        public async Task<JsonResult> ProfileSummaryData()
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            var user = await _userService.QueryUser()
                .Include(u => u.Conversations.Select(c => c.ConversationOptions))
                .SingleAsync(u => u.Id == userId);

            if (user.IsDeactivated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
                throw new ApplicationException("Account deactivated. Please contact support.");
            }

            var profileDto = new ProfileSummaryDto
            {
                Settings = new UserSettings
                {
                    ShowWelcome = user.ShowWelcome
                },
                User = new UserViewDto
                {
                    Gender = user.Gender,
                    Name = user.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(user.ProfilePhotoUrl),
                    UserId = user.Id,
                    Interest = user.Interest,
                    Birthday = user.Birthday
                },
                HasNewMessages = user.Conversations
                    .SelectMany(c => c.ConversationOptions)
                    .Where(opt => opt.UserId == userId)
                    .Any(opt => opt.HasNewMessages)
            };

            return Json(profileDto, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ProfileData(Guid id)
        {
            var authId = this.User.Identity.GetUserIdGuid();
            var user = await _userService.QueryUser().SingleAsync(u => u.Id == id);

            if (authId != id)
            {
                await _userService.IncrementViewCount(id);
            }

            var userDto = new UserFullViewDto
            {
                AlbumPhotos = JsonConvert.DeserializeObject<List<string>>(user.AlbumPhotoUrls)
                    .Select(PhotoUrlService.GetPhotoDto).ToList(),
                Birthday = user.Birthday,
                Description = user.Description,
                Gender = user.Gender,
                Interest = user.Interest,
                Name = user.Name,
                ProfilePhoto = PhotoUrlService.GetPhotoDto(user.ProfilePhotoUrl),
                UserId = user.Id,
                Location = user.Location,
                CanEdit = authId.HasValue ? authId.Value == id : false,
                IsOnline = _onlineUserService.GetOnlineUsers().Any(u => u.Id == user.Id)
            };

            return Json(userDto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfile(MeProfileViewModel model)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            await _userService.UpdateProfile(userId, model.Name,
                model.Description, model.Location, model.Birthday, model.Gender, model.Interest);

            _hubContext.Clients.User(userId.ToString())
                .ProfileUpdated(userId, model.Name, model.Description, model.Location, model.Birthday, model.Gender);

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<JsonResult> UploadProfilePhoto()
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            var user = await _userService.QueryUser()
                .SingleAsync(u => u.Id == userId);

            if (this.HttpContext.Request.Files == null
                || this.HttpContext.Request.Files.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = "File not found."
                });
            }

            var photo = this.HttpContext.Request.Files[0];
            string fileUrl = await this.SaveFile(userId, "profile_" + photo.FileName, photo.InputStream);

            if (user.ProfilePhotoUrl != null)
            {
                var result = await _fileStorageService.DeleteFile(user.ProfilePhotoUrl);
            }

            await _userService.SetProfilePhotoUrl(userId, fileUrl);

            _hubContext.Clients.All
                .ProfilePhotoChanged(userId, PhotoUrlService.GetPhotoDto(fileUrl));

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<JsonResult> UploadPhotoForAlbum()
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;

            if (this.HttpContext.Request.Files == null
                || this.HttpContext.Request.Files.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = "File not found."
                });
            }

            var photo = this.HttpContext.Request.Files[0];
            string fileUrl = await this.SaveFile(userId, photo.FileName, photo.InputStream);
            await _userService.AddAlbumPhotoUrl(userId, fileUrl);

            _hubContext.Clients.All
                .GalleryPhotoUploaded(userId, PhotoUrlService.GetPhotoDto(fileUrl));

            return Json(new
            {
                success = true
            });
        }

        private async Task<string> SaveFile(Guid userId, string fileName, Stream inputStream)
        {
            var name = this.RemoveSpecialCharacters(Path.GetFileNameWithoutExtension(fileName));
            string extension = Path.GetExtension(fileName);
            return await _fileStorageService.SaveFile(userId, name, extension, inputStream);
        }

        [HttpPost]
        public async Task<ActionResult> DeletePhotosFromAlbum(Guid userId, List<string> photoUrls)
        {
            var authUserId = this.User.Identity.GetUserIdGuid().Value;
            if (authUserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            await Task.WhenAll(photoUrls.Select(url => _fileStorageService.DeleteFile(url)));
            await _userService.RemovePhotosFromAlbum(userId, photoUrls);
            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> StopShowingWelcome()
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            await _userService.StopShowingWelcome(userId);
            return new EmptyResult();
        }

        private string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
