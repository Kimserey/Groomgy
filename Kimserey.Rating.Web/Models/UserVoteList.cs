using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public class UserVoteList
    {
        public Guid UserId { get; set; }
        public List<Vote> Votes { get; set; }

        public UserVoteList() { }

        public UserVoteList(Guid userId)
        {
            this.UserId = userId;
        }

        public void AddVote(Vote vote)
        {
            if (this.Votes.Any(v => v.RatedByUserId == vote.RatedByUserId
                   && (vote.RatedOn - v.RatedOn).TotalHours < 3))
            {
                throw new ArgumentException("User is not allowed to vote");
            }
            this.Votes.Add(vote);
        }

        public void DeleteVote(Guid voteId, Guid deletorId)
        {
            var vote = this.Votes.FirstOrDefault(v => v.Id == voteId);
            if (vote == null)
            {
                return;
            }

            if (vote.RatedByUserId != deletorId && this.UserId != deletorId)
            {
                throw new ArgumentException("User is not allowed to delete vote");
            }

            vote.IsDeleted = true;
        }
    }
}
