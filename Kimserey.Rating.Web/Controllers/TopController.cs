using Kimserey.Rating.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Kimserey.Rating.Web.Dto;
using Kimserey.Rating.Web.Persistence.Entities;
using Kimserey.Rating.Web.Models;

namespace Kimserey.Rating.Web.Controllers
{
    public class TopController : Controller
    {
        private IVoteService _voteService;
        private IConversationService _conversationService;
        private IUserService _userService;

        public TopController(IUserService userService,
            IVoteService voteService,
            IConversationService conversationService)
        {
            _userService = userService;
            _voteService = voteService;
            _conversationService = conversationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TileTemplate()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMostViewedLadyData()
        {
            var mostViewedLady = await Task.WhenAll((await _userService.QueryUser()
                .Where(u => !u.IsDeactivated)
                .Where(u => u.Gender == Gender.Woman)
                .Where(u => u.CountProfileView > 0)
                .OrderByDescending(u => u.CountProfileView)
                .Take(10)
                .ToListAsync())
                .Select(u => new UserValue
                {
                    UserId = u.Id,
                    Value = u.CountProfileView
                })
                .Select(u => ConvertToUserRankDto(u, "persons viewed this profile!")));

            return Json(mostViewedLady, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMostViewedManData()
        {
            var mostViewedLady = await Task.WhenAll((await _userService.QueryUser()
                .Where(u => !u.IsDeactivated)
                .Where(u => u.Gender == Gender.Man)
                .Where(u => u.CountProfileView > 0)
                .OrderByDescending(u => u.CountProfileView)
                .Take(10)
                .ToListAsync())
                .Select(u => new UserValue
                {
                    UserId = u.Id,
                    Value = u.CountProfileView
                })
                .Select(u => ConvertToUserRankDto(u, "persons viewed this profile!")));

            return Json(mostViewedLady, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMostVotesLadyData()
        {
            var mostVotesLady = await Task.WhenAll((await this.OrderByDescAndTakeAsync<VoteEntity>(
                _voteService.QueryVote()
                .Where(i => !i.RatedOnUser.IsDeactivated)
                .Where(i => i.RatedOnUser.Gender == Gender.Woman)
                .GroupBy(i => i.RatedOnUserId), 10))
                .Select(u => this.ConvertToUserRankDto(u, "votes received!")));

            return Json(mostVotesLady, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMostVotesManData()
        {
            var mostVotesMan = await Task.WhenAll((await this.OrderByDescAndTakeAsync<VoteEntity>(
                _voteService.QueryVote()
                .Where(i => !i.RatedOnUser.IsDeactivated)
                .Where(i => i.RatedOnUser.Gender == Gender.Man)
                .GroupBy(i => i.RatedOnUserId), 10))
                .Select(u => this.ConvertToUserRankDto(u, "votes received!")));

            return Json(mostVotesMan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetVoteAddictsManData()
        {
            var voteAddictsMan = await Task.WhenAll((await this.OrderByDescAndTakeAsync<VoteEntity>(_voteService.QueryVote()
                .Where(i => !i.RatedByUser.IsDeactivated)
                .Where(i => i.RatedByUser.Gender == Gender.Man)
                .GroupBy(i => i.RatedByUserId), 5))
                .Select(u => this.ConvertToUserRankDto(u, "votes sent!")));

            return Json(voteAddictsMan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetVoteAddictsLadyData()
        {
            var voteAddictsLady = await Task.WhenAll((await this.OrderByDescAndTakeAsync<VoteEntity>(
                _voteService.QueryVote()
                .Where(i => !i.RatedByUser.IsDeactivated)
                .Where(i => i.RatedByUser.Gender == Gender.Woman)
                .GroupBy(i => i.RatedByUserId), 5))
                .Select(u => this.ConvertToUserRankDto(u, "votes sent!")));

            return Json(voteAddictsLady, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMessageAddictsManData()
        {
            var messageAddictsMan = await Task.WhenAll((await this.OrderByDescAndTakeAsync<MessageEntity>(_conversationService
                .QueryConversation()
                .SelectMany(c => c.Messages)
                .Where(i => i.SentByUser.Gender == Gender.Man)
                .GroupBy(m => m.SentByUserId), 5))
                .Select(u => this.ConvertToUserRankDto(u, "messages sent!")));

            return Json(messageAddictsMan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetMessageAddictsLadyData()
        {
            var messageAddictsLady = await Task.WhenAll((await this.OrderByDescAndTakeAsync<MessageEntity>(_conversationService
               .QueryConversation()
               .SelectMany(c => c.Messages)
               .Where(i => i.SentByUser.Gender == Gender.Woman)
               .GroupBy(m => m.SentByUserId), 5))
               .Select(u => this.ConvertToUserRankDto(u, "messages sent!")));

            return Json(messageAddictsLady, JsonRequestBehavior.AllowGet);
        }

        private Task<List<UserValue>> OrderByDescAndTakeAsync<T>(IQueryable<IGrouping<Guid, T>> query, int take)
        {
            return query.Select(g => new UserValue { UserId = g.Key, Value = g.Count() })
                .Where(i => i.Value > 0)
                .OrderByDescending(i => i.Value)
                .Take(take)
                .ToListAsync();
        }

        private async Task<UserRankDto> ConvertToUserRankDto(UserValue userValue, string postfix)
        {
            var user = await _userService.QueryUser().SingleAsync(u => u.Id == userValue.UserId);
            return new UserRankDto
            {
                Value = userValue.Value + " " + postfix,
                User = new UserViewDto
                {
                    Gender = user.Gender,
                    Name = user.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(user.ProfilePhotoUrl),
                    UserId = userValue.UserId
                }
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _conversationService.Dispose();
                _userService.Dispose();
                _voteService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
