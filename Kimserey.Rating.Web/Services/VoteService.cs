using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper;

namespace Kimserey.Rating.Web.Services
{
    public class VoteService : IVoteService
    {
        private IRepository _repository;

        static VoteService()
        {
            Mapper.CreateMap<VoteEntity, Vote>();
        }

        public VoteService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Vote(Guid voteId, int rate, string comment, Guid ratedByUserId, Guid ratedOnUserId, DateTime ratedOn)
        {
            var userVoteList = await _repository.GetUserVoteList(ratedOnUserId);
            var vote = new Vote(voteId,
                rate,
                comment,
                ratedByUserId,
                ratedOnUserId,
                ratedOn);

            userVoteList.AddVote(vote);
            await _repository.SaveVote(vote);
        }

        public IQueryable<VoteEntity> QueryVote()
        {
            return _repository.QueryVote();
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
