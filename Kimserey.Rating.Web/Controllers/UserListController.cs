using Kimserey.Rating.Web.Dto;
using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Infrastructure;
using Kimserey.Rating.Web.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using System.Linq.Expressions;
using Kimserey.Rating.Web.Persistence.Entities;
using Kimserey.Rating.Web.Models;

namespace Kimserey.Rating.Web.Controllers
{
    public class UserListController : Controller
    {
        private IUserService _userService;
        private IOnlineUserService _onlineUserService;

        public UserListController(IUserService userService, IOnlineUserService onlineUserService)
        {
            _userService = userService;
            _onlineUserService = onlineUserService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserTileList()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UserTileListData(List<Guid> skipIds, int take, Gender gender)
        {
            Expression<Func<UserEntity, bool>> skipPredicate = user => true;
            if (skipIds != null)
            {
                skipPredicate = user => !skipIds.Contains(user.Id);
            }

            //Filter only if man or woman gender is specified
            Expression<Func<UserEntity, bool>> genderPredicate = user => true;
            if (gender != Gender.NotSpecified)
            {
                genderPredicate = user => user.Gender == gender;
            }

            var users = (await _userService.QueryUser()
                .Where(skipPredicate)
                .Where(genderPredicate)
                .Where(u => !u.IsDeactivated)
                .OrderBy(u => Guid.NewGuid())
                .Take(take)
                .Select(u => new
                {
                    Gender = u.Gender,
                    Name = u.Name,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                    UserId = u.Id,
                    Interest = u.Interest
                })
                .ToListAsync())
                .Select(u => new UserViewDto
                {
                    Gender = u.Gender,
                    Name = u.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(u.ProfilePhotoUrl),
                    UserId = u.UserId,
                    Interest = u.Interest
                });
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NameList()
        {
            return View();
        }

        public async Task<JsonResult> NameListData()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var onlineUserIds = (await _onlineUserService
                .GetOnlineUsers()
                .Select(u => u.Id)
                .ToListAsync())
                .Concat(Consts.FixedOnlineUsers)
                .Concat(new[] { new Guid(this.User.Identity.GetUserId()) })
                .Distinct()
                .ToList();

            List<UserNameDto> users = await _userService.QueryUser()
                .Where(u => !u.IsDeactivated)
                .Select(u => new UserNameDto
                {
                    Name = u.Name,
                    Gender = u.Gender,
                    UserId = u.Id,
                    IsOnline = onlineUserIds.Any(i => i == u.Id)
                })
                .ToListAsync();

            return Json(users, JsonRequestBehavior.AllowGet);
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
