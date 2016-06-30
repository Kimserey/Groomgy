using Kimserey.Rating.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Dto;
using Newtonsoft.Json;

namespace Kimserey.Rating.Web.Controllers
{
    public class MatchController : Controller
    {
        private IUserService _userService;

        public MatchController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<JsonResult> GetMatch()
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            var user = await _userService.QueryUser().SingleAsync(u => u.Id == userId);

            var matches = await _userService
                .QueryUser()
                .Where(u => u.Id != userId)
                .Where(u => u.Gender == user.Interest)
                .OrderBy(u => Guid.NewGuid())
                .Take(2)
                .ToListAsync();

            var dtos = matches.Select(m =>
                new UserFullViewDto
                {
                    AlbumPhotos = JsonConvert.DeserializeObject<List<string>>(m.AlbumPhotoUrls)
                        .Select(PhotoUrlService.GetPhotoDto).ToList(),
                    Birthday = m.Birthday,
                    Description = m.Description,
                    Gender = m.Gender,
                    Name = m.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(m.ProfilePhotoUrl),
                    UserId = m.Id,
                    Location = m.Location
                }).ToList();

            return Json(dtos, JsonRequestBehavior.AllowGet);
        }
    }
}
