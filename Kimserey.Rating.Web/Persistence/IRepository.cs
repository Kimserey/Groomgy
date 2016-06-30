using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Kimserey.Rating.Web.Persistence
{
    public interface IRepository : IDisposable
    {
        Task SaveUser(User user);
        Task SaveVote(Vote vote);
        Task SaveConversation(Conversation conversation);
        Task<UserVoteList> GetUserVoteList(Guid userId);
        IQueryable<UserEntity> QueryUser();
        IQueryable<VoteEntity> QueryVote();
        IQueryable<ConversationEntity> QueryConversation();
    }
}