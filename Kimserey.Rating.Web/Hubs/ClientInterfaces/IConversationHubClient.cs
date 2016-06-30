using Kimserey.Rating.Web.Dto;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Hubs.ClientInterfaces
{
    public interface IConversationHubClient
    {
        void MessageReceived(Guid conversationId, 
            UserViewDto user,
            string text, 
            DateTime sentOn);
        void HasNewMessages(Guid userId);
        void MarkedAsRead(Guid conversationId);
    }
}
