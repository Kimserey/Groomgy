using Kimserey.Rating.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Hubs.ClientInterfaces
{
    public interface IVoteHubClient
    {
        void VoteReceived(Guid voteId, Guid votedForUserId, UserViewDto voteByUser, int rate, string comment, DateTime date);
    }
}
