using Kimserey.Rating.Web.Services;
using Kimserey.Rating.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Kimserey.Rating.Web.Dto;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.SignalR.Infrastructure;
using Kimserey.Rating.Web.Hubs;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Microsoft.AspNet.SignalR;

namespace Kimserey.Rating.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class MessageController : Controller
    {
        private IConversationService _conversationService;
        private IUserService _userService;
        private IHubContext<IConversationHubClient> _conversationHub;
        private IOnlineUserService _onlineUserService;

        public MessageController(IUserService userService,
            IOnlineUserService onlineUserService,
            IConversationService conversationService,
            IHubContext<IConversationHubClient> conversationHub)
        {
            _onlineUserService = onlineUserService;
            _conversationService = conversationService;
            _userService = userService;
            _conversationHub = conversationHub;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConversationList()
        {
            return View();
        }

        public async Task<JsonResult> ConversationListData()
        {
            var authUserId = this.User.Identity.GetUserIdGuid().Value;
            var userConversations = (await _userService
                .QueryUser()
                .Include(u => u.Conversations)
                .Include(u => u.Conversations.Select(c => c.Users))
                .SingleAsync(u => u.Id == authUserId))
                .Conversations
                .Where(c => !c.ConversationOptions.Single(opt => opt.UserId == authUserId).IsDeleted);

            List<ConversationPreviewDto> listDto = new List<ConversationPreviewDto>();
            foreach (var conv in userConversations)
            {
                var user = conv.Users.First(u => u.Id != authUserId);
                var userDto = new UserViewDto
                {
                    Gender = user.Gender,
                    Name = user.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(user.ProfilePhotoUrl),
                    UserId = user.Id
                };

                var lastMessage = conv.Messages
                    .OrderByDescending(m => m.SentOn)
                    .Where(m => m.SentByUserId != authUserId)
                    .FirstOrDefault();

                MessageDto lastMessageDto = lastMessage != null
                    ? new MessageDto
                    {
                        SentByUserId = lastMessage.SentByUserId,
                        SentOn = lastMessage.SentOn,
                        Text = lastMessage.Text
                    } : null;

                listDto.Add(new ConversationPreviewDto
                {
                    ConversationId = conv.Id,
                    Message = lastMessageDto,
                    User = userDto,
                    HasNewMessages = conv
                        .ConversationOptions
                        .Single(u => u.UserId == authUserId)
                        .HasNewMessages
                });
            }

            return Json(listDto, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Conversation()
        {
            return View();
        }

        public async Task<JsonResult> ConversationData(Guid id)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;

            var conversation = await _conversationService
                .QueryConversation()
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .SingleAsync(c => c.Id == id);

            var interlocutorId = conversation.Users.First(u => u.Id != userId).Id;

            ConversationDto conversationDto = new ConversationDto
            {
                ConversationId = conversation.Id,
                Title = conversation.Users.Single(u => u.Id != userId).Name,
                Messages = conversation.Messages.OrderByDescending(m => m.SentOn)
                .Take(20)
                .Select(m => new MessageDto
                {
                    SentByUserId = m.SentByUserId,
                    SentOn = m.SentOn,
                    Text = m.Text,
                    IsSentByUser = m.SentByUserId == userId
                })
                .OrderBy(m => m.SentOn)
                .ToList(),
                Users = conversation.Users.Select(u => new UserViewDto
                {
                    Gender = u.Gender,
                    Name = u.Name,
                    ProfilePhoto = PhotoUrlService.GetPhotoDto(u.ProfilePhotoUrl),
                    UserId = u.Id
                }).ToList(),
                IsInterlocutorOnline = _onlineUserService.GetOnlineUsers()
                    .Any(u => u.Id == interlocutorId),
                InterlocutorId = interlocutorId
            };

            return Json(conversationDto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> StartConversation(Guid id)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            Guid conversationId;

            var conversation = (await _userService.QueryUser()
                .SingleAsync(u => u.Id == userId))
                .Conversations
                .FirstOrDefault(c => c.Users.Any(u => u.Id == id));

            if (conversation == null)
            {
                conversationId = Guid.NewGuid();
                await _conversationService.StartConversation(conversationId, userId, id);
            }
            else
            {
                conversationId = conversation != null ? conversation.Id : Guid.NewGuid();
                await _conversationService.ReOpenConversation(conversationId, userId, id);
            }

            return Json(new { conversationId = conversationId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Send(MessageViewModel model)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            await _conversationService.SendMessage(
                model.ConversationId,
                userId,
                model.SentOn,
                model.Text);

            var interlocutorUserIds = await _conversationService.QueryConversation()
                .Include(c => c.Users)
                .Where(c => c.Id == model.ConversationId)
                .SelectMany(c => c.Users)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var id in interlocutorUserIds
                .Where(interlocId => interlocId != userId))
            {
                var userDto = await _userService.QueryUser()
                    .SingleAsync(u => u.Id == userId);

                _conversationHub
                    .Clients
                    .User(id.ToString())
                    .MessageReceived(model.ConversationId,
                    new UserViewDto
                    {
                        Gender = userDto.Gender,
                        Name = userDto.Name,
                        ProfilePhoto = PhotoUrlService.GetPhotoDto(userDto.ProfilePhotoUrl),
                        UserId = userDto.Id
                    },
                    model.Text,
                    model.SentOn);

                _conversationHub
                    .Clients
                    .User(id.ToString())
                    .HasNewMessages(id);
            }

            return Json(new MessagePostedViewModel
            {
                UserId = userId,
                Text = model.Text,
                SentOn = model.SentOn
            });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid conversationId)
        {
            var userId = this.User.Identity.GetUserIdGuid();
            await _conversationService.Delete(conversationId, userId.Value);
            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> MarkConversationAsRead(Guid conversationId)
        {
            var userId = this.User.Identity.GetUserIdGuid().Value;
            await _conversationService.MarkConversationAsRead(conversationId, userId);

            _conversationHub.Clients
                .User(userId.ToString())
                .MarkedAsRead(conversationId);

            return new EmptyResult();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _conversationService.Dispose();
                _userService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
