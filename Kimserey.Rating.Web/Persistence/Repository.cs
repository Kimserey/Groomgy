using AutoMapper;
using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence
{

    public class Repository : IRepository
    {
        private PeopleDbContext _dbContext;

        static Repository()
        {
            Mapper.CreateMap<ConversationOption, ConversationOptionEntity>();
            Mapper.CreateMap<Conversation, ConversationEntity>()
                .ForMember(entity => entity.Users, x => x.Ignore())
                .ForMember(entity => entity.ConversationOptions, x => x.Ignore());
            Mapper.CreateMap<Message, MessageEntity>();

            Mapper.CreateMap<Vote, VoteEntity>();

            Mapper.CreateMap<User, UserEntity>()
                .ForMember(entity => entity.AlbumPhotoUrls, x => x.Ignore())
                .AfterMap((user, entity) => entity.AlbumPhotoUrls = JsonConvert.SerializeObject(user.AlbumPhotoUrls));
        }

        public Repository()
        {
            _dbContext = new PeopleDbContext();
        }

        public Task SaveUser(User user)
        {
            return this.AddOrUpdate<User, UserEntity>(_dbContext.UserEntities, user.Id, user);
        }

        public Task SaveVote(Vote vote)
        {
            return this.AddOrUpdate<Vote, VoteEntity>(_dbContext.VoteEntities, vote.Id, vote);
        }

        public async Task SaveConversation(Conversation conversation)
        {
            ConversationEntity entity = _dbContext
                .ConversationEntities
                .Include(c => c.Messages)
                .Include(c => c.ConversationOptions)
                .FirstOrDefault(e => e.Id == conversation.Id);

            if (entity != null)
            {
                var toAdd = conversation.Messages.Select(m => m.Id).Except(entity.Messages.Select(m => m.Id));
                foreach (var messageId in toAdd.ToList())
                {
                    var message = Mapper.Map<MessageEntity>(conversation.Messages
                        .Single(m => m.Id == messageId));

                    entity.Messages.Add(message);
                }

                var optionIds = entity.ConversationOptions.Select(ec => ec.Id).ToList();
                foreach (var option in entity.ConversationOptions)
                {
                    Mapper.Map<ConversationOption, ConversationOptionEntity>(
                        conversation.ConversationOptions.Single(c => c.Id == option.Id), option);
                }
            }
            else
            {
                entity = Mapper.Map<ConversationEntity>(conversation);
                _dbContext.ConversationEntities.Add(entity);
                await _dbContext.SaveChangesAsync();

                entity = _dbContext.ConversationEntities
                    .Include(c => c.Users)
                    .Include(c => c.ConversationOptions)
                    .FirstOrDefault(e => e.Id == conversation.Id);
                conversation
                    .UsersIds
                    .ForEach(userId => entity.Users.Add(_dbContext.UserEntities.Find(userId)));
                conversation
                    .ConversationOptions
                    .ForEach(option => entity.ConversationOptions.Add(Mapper.Map<ConversationOptionEntity>(option)));
            }

            await _dbContext.SaveChangesAsync();
        }

        private Task AddOrUpdate<T, TEntity>(DbSet<TEntity> dbSet, Guid entityId, T valueToAddOrModify)
            where TEntity : Entity
        {
            TEntity entity = dbSet.FirstOrDefault(e => e.Id == entityId);
            if (entity != null)
            {
                Mapper.Map<T, TEntity>(valueToAddOrModify, entity);
            }
            else
            {
                entity = Mapper.Map<TEntity>(valueToAddOrModify);
                dbSet.Add(entity);
            }
            return _dbContext.SaveChangesAsync();
        }

        public IQueryable<UserEntity> QueryUser()
        {
            return new PeopleDbContext()
                .UserEntities
                .AsQueryable();
        }

        public IQueryable<VoteEntity> QueryVote()
        {
            return new PeopleDbContext()
                .VoteEntities
                .AsQueryable();
        }

        public IQueryable<ConversationEntity> QueryConversation()
        {
            return new PeopleDbContext()
                .ConversationEntities
                .AsQueryable();
        }

        public async Task<UserVoteList> GetUserVoteList(Guid userId)
        {
            var userEntity = await this.QueryUser()
                .Include(u => u.ReceivedVotes)
                .SingleAsync(u => u.Id == userId);

            var userList = new UserVoteList(userId);
            userList.Votes = userEntity
                .ReceivedVotes
                .Select(v => new Vote
                {
                    Comment = v.Comment,
                    Id = v.Id,
                    IsDeleted = v.IsDeleted,
                    Rate = v.Rate,
                    RatedByUserId = v.RatedByUserId,
                    RatedOn = v.RatedOn,
                    RatedOnUserId = v.RatedOnUserId
                }).ToList();
            return userList;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
