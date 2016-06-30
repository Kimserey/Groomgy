using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IConversationService : IDisposable
    {
        Task StartConversation(Guid conversationId, Guid startedByUserId, Guid withUserId);
        Task ReOpenConversation(Guid conversationId, Guid startedByUserId, Guid withUserId);
        Task SendMessage(Guid conversationId, Guid sentBy, DateTime sentOn, string text);
        Task Delete(Guid conversationId, Guid userId);
        Task MarkConversationAsRead(Guid conversationId, Guid userId);
        IQueryable<ConversationEntity> QueryConversation();
    }

}
