using Kimserey.Rating.Web.Dto;
using Kimserey.Rating.Web.EmailModels;
using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace Kimserey.Rating.Web.Controllers
{
    public class WeeklyEmailController : Controller
    {
        private IEmailService _emailService;
        private IUserService _userService;

        public WeeklyEmailController(IEmailService emailService, IUserService userService)
        {
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Check(string weeklyEmailKey, Guid userId, string checkEmail)
        {
            if (weeklyEmailKey != Consts.WeeklyEmailKey) { throw new UnauthorizedAccessException(); }

            var users = await this.GetUserWeeklyData().ToListAsync();
            var userEmail = _userService.QueryUser().Where(u => u.Id == userId).Single().Email;
            var email = this.BuildGroomgyWeeklyEmails(users).Where(e => e.Email == userEmail).First();
            email.Email = checkEmail;

            await _emailService.SendWeeklyEmail(new[] { email });
            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> Send(string weeklyEmailKey)
        {
            if (weeklyEmailKey != Consts.WeeklyEmailKey) { throw new UnauthorizedAccessException(); }

            var users = await this.GetUserWeeklyData().ToListAsync();
            var emails = this.BuildGroomgyWeeklyEmails(users);

            await _emailService.SendWeeklyEmail(emails);
            return new EmptyResult();
        }

        private IQueryable<UserWeeklyData> GetUserWeeklyData()
        {
            return _userService.QueryUser()
                    .Select(u => new UserWeeklyData
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Name = u.Name,
                        Gender = u.Gender,
                        Photo = u.ProfilePhotoUrl,
                        UnreadMessages = u.Conversations
                            .Where(c => c.ConversationOptions.Any(opt => opt.UserId == u.Id && opt.HasNewMessages))
                            .Select(c => new MessageWeeklyData
                            {
                                Text = c.Messages
                                    .Where(m => m.SentByUserId != u.Id)
                                    .OrderByDescending(m => m.SentOn)
                                    .FirstOrDefault()
                                    .Text,
                                Interlocutor = new Interlocutor
                                {
                                    Id = c.Users.FirstOrDefault(_ => _.Id != u.Id).Id,
                                    Name = c.Users.FirstOrDefault(_ => _.Id != u.Id).Name,
                                    Photo = c.Users.FirstOrDefault(_ => _.Id != u.Id).ProfilePhotoUrl
                                }
                            })
                    });
        }

        private IEnumerable<GroomgyWeeklyEmail> BuildGroomgyWeeklyEmails(IEnumerable<UserWeeklyData> users)
        {
            var emails = new List<GroomgyWeeklyEmail>();
            
            foreach(var user in users)
            {
                GroomgyWeeklyEmail weekly = new GroomgyWeeklyEmail
                {
                    Email = user.Email,
                    User = user.Name,
                    Conversations = user.UnreadMessages.Select(m => new ConversationGroomgyWeeklyEmail
                    {
                        Image = PhotoUrlService.GetPhotoDto(m.Interlocutor.Photo).Small,
                        Message = m.Text,
                        Name = m.Interlocutor.Name
                    }).ToArray(),
                    Members = users.Where(u => u.Gender != user.Gender)
                    .OrderBy(_ => new Guid())
                    .Take(4)
                        .Select(u => new MemberSuggestedGroomgyWeeklyEmail
                        {
                            Image = PhotoUrlService.GetPhotoDto(u.Photo).Small,
                            Url = string.Format("{0}profile/{1}", Consts.SiteUrl, u.Id)
                        }).ToArray()
                };

                if (weekly.Conversations.Count() == 0 && weekly.Members.Count() == 0)
                {
                    continue;
                }
                emails.Add(weekly);
            }

            return emails;
        }
    }

    public class UserWeeklyData
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string Photo { get; set; }
        public IEnumerable<MessageWeeklyData> UnreadMessages { get; set; }
    }

    public class MessageWeeklyData
    {
        public string Text { get; set; }
        public Interlocutor Interlocutor { get; set; }
    }

    public class Interlocutor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
    }

}
