using Kimserey.Rating.Web.Hubs;
using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Kimserey.Rating.Web.Persistence;
using System.Data.Entity;

namespace Kimserey.Rating.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View(new GlobalVariableViewModel
            {
                SiteUrl = Consts.SiteUrl,
                GoogleAnalyticsTrackingId = Consts.GoogleAnalyticsTrackingId
            });
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult Loader()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public async Task<bool> GetEmailNotificationEnabled(string email)
        {
            Guid userId = await this.GetUserId(email);
            var user = await _userService.QueryUser().SingleAsync(u => u.Id == userId);
            return user.EmailNotificationEnabled;
        }

        [HttpPost]
        public async Task<ActionResult> EnableEmailNotification(string email, bool isEnabled)
        {
            Guid userId = await this.GetUserId(email);
            await _userService.EnableEmailNotification(userId, isEnabled);
            return new EmptyResult();
        }

        private async Task<Guid> GetUserId(string email)
        {
            return string.IsNullOrEmpty(email)
                ? this.User.Identity.GetUserIdGuid().Value
                : (await _userService.QueryUser().SingleAsync(u => u.Email == email)).Id;
        }
    }
}