using AutoMapper;
using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public class ConversationService : IConversationService
    {
        private IRepository _repository;

        static ConversationService()
        {
            Mapper.CreateMap<ConversationEntity, Conversation>();
            Mapper.CreateMap<ConversationOptionEntity, ConversationOption>();
            Mapper.CreateMap<MessageEntity, Message>();
        }

        public ConversationService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task StartConversation(Guid conversationId, Guid startedByUserId, Guid withUserId)
        {
            await _repository.SaveConversation(new Conversation(conversationId, new[] { startedByUserId, withUserId }));
        }

        public async Task ReOpenConversation(Guid conversationId, Guid startedByUserId, Guid withUserId)
        {
            var conversationEntity = await _repository.QueryConversation()
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversationEntity == null)
            {
                throw new ArgumentException("Conversation does not exists.");
            }

            var conversation = this.MapToConversation(conversationEntity);
            conversation.ReOpen(startedByUserId, withUserId);
            await _repository.SaveConversation(conversation);

        }

        public async Task SendMessage(Guid conversationId, Guid sentBy, DateTime sentOn, string text)
        {
            var conversationEntity = await _repository.QueryConversation()
                .Include(c => c.ConversationOptions)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversationEntity == null)
            {
                throw new ArgumentException("Conversation does not exists.");
            }

            Conversation conversation = this.MapToConversation(conversationEntity);
            conversation.AddMessage(sentBy, sentOn, text);
            await _repository.SaveConversation(conversation);
        }

        public async Task Delete(Guid conversationId, Guid userId)
        {
            var conversationEntity = await _repository.QueryConversation()
                   .SingleAsync(c => c.Id == conversationId);

            var conversation = this.MapToConversation(conversationEntity);

            conversation.Delete(userId);
            await _repository.SaveConversation(conversation);
        }

        public async Task MarkConversationAsRead(Guid conversationId, Guid userId)
        {
            var conversationEntity = await _repository.QueryConversation()
                      .SingleAsync(c => c.Id == conversationId);

            var conversation = this.MapToConversation(conversationEntity);

            conversation.MarkConversationAsRead(userId);
            await _repository.SaveConversation(conversation);
        }

        private Conversation MapToConversation(ConversationEntity entity)
        {
            var conversation = Mapper.Map<Conversation>(entity);
            conversation.UsersIds = entity.Users.Select(u => u.Id).ToList();
            return conversation;
        }

        public IQueryable<ConversationEntity> QueryConversation()
        {
            return _repository.QueryConversation();
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
