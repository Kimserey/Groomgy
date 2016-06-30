using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Kimserey.Rating.Web.Dto;
using Microsoft.AspNet.SignalR;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Kimserey.Rating.Web.Persistence;

namespace Kimserey.Rating.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class VoteController : Controller
    {
        private IVoteService _voteService;
        private IUserService _userService;
        private IHubContext<IVoteHubClient> _voteHub;
        private IEmailService _emailService;

        public VoteController(IVoteService voteService,
            IUserService userService,
            IHubContext<IVoteHubClient> voteHub,
            IEmailService emailService)
        {
            _voteService = voteService;
            _userService = userService;
            _voteHub = voteHub;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public ActionResult List()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<JsonResult> ListData(Guid id)
        {
            var userId = this.User.Identity.GetUserIdGuid();

            var user = await _userService
                .QueryUser()
                .Include(u => u.ReceivedVotes)
                .Include(u => u.ReceivedVotes.Select(v => v.RatedByUser))
                .SingleAsync(u => u.Id == id);

            var votes = user.ReceivedVotes
                .Select(v => new VoteDto
                 {
                     VoteId = v.Id,
                     Comment = v.Comment,
                     Rate = v.Rate,
                     RatedOn = v.RatedOn,
                     VotedByUser = new UserViewDto
                     {
                         Gender = v.RatedByUser.Gender,
                         Name = v.RatedByUser.Name,
                         ProfilePhoto = PhotoUrlService.GetPhotoDto(v.RatedByUser.ProfilePhotoUrl),
                         UserId = v.RatedByUser.Id
                     }
                 });

            var voteListDto = new VoteListDto
            {
                CanVote = !userId.HasValue || userId.Value != id,
                Votes = votes.ToList()
            };

            return Json(voteListDto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> VoteForUser(VoteViewModel model)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            var voteId = Guid.NewGuid();

            var latestVote = _userService.QueryUser()
                .Include(u => u.ReceivedVotes)
                .Where(u => u.Id == model.RatedOnUserId)
                .SelectMany(u => u.ReceivedVotes)
                .OrderByDescending(v => v.RatedOn)
                .FirstOrDefault(v => v.RatedByUserId == userId);

            if (latestVote != null && (model.VotedOn - latestVote.RatedOn).TotalHours < 3)
            {
                return Json(new { success = false, error = "You can only vote every 3 hours." });
            }

            await _voteService.Vote(voteId, model.Rate, model.Comment, userId, model.RatedOnUserId, model.VotedOn);

            var user = await _userService.QueryUser().SingleAsync(u => u.Id == userId);
            UserViewDto userView = new UserViewDto
            {
                Gender = user.Gender,
                Name = user.Name,
                ProfilePhoto = PhotoUrlService.GetPhotoDto(user.ProfilePhotoUrl),
                UserId = user.Id
            };

            var ratedOnUser = await _userService.QueryUser()
                .SingleAsync(u => u.Id == model.RatedOnUserId);

            if (ratedOnUser.EmailNotificationEnabled)
            {
                await _emailService.SendVoteReceived(ratedOnUser.Email, model.RatedOnUserId, userView.Name);
            }

            _voteHub.Clients.All.VoteReceived(voteId,
                model.RatedOnUserId,
                userView,
                model.Rate,
                model.Comment,
                model.VotedOn);

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userService.Dispose();
                _voteService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
