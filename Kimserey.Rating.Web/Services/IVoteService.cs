using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IVoteService: IDisposable
    {
        Task Vote(Guid voteId, int rate, string comment, Guid ratedByUserId, Guid ratedOnUserId, DateTime ratedOn);
        IQueryable<VoteEntity> QueryVote();
    }
}
