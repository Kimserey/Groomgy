using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public List<ConversationOption> ConversationOptions { get; set; }
        public List<Guid> UsersIds { get; set; }
        public List<Message> Messages { get; set; }

        public Conversation() { }

        public Conversation(Guid id, IEnumerable<Guid> interlocutorUserIds)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty guid");
            }

            if (interlocutorUserIds.Count() < 2)
            {
                throw new ArgumentException("Conversation must have a least 2 interlocutors");
            }

            this.Id = id;
            this.UsersIds = interlocutorUserIds.ToList();
            this.ConversationOptions = new[] {
             new ConversationOption {
                 Id = Guid.NewGuid(),
                 IsDeleted = false,
                 UserId = interlocutorUserIds.First()
             },
             new ConversationOption {
                 Id = Guid.NewGuid(),
                 IsDeleted = false,
                 UserId = interlocutorUserIds.Skip(1).First()
             }}.ToList();
            this.Messages = new List<Message>();
        }

        public void ReOpen(params Guid[] interlocutors)
        {
            foreach (var option in this.ConversationOptions.Where(c => interlocutors.Contains(c.UserId)))
            {
                option.IsDeleted = false;
            }
        }

        public void AddMessage(Guid sentByUserId, DateTime sentOn, string text)
        {
            if (sentOn < Consts.MinDate)
            {
                throw new ArgumentException("sentOn date is invalid");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text cannot be null or empty");
            }

            if (!this.ConversationOptions.Select(i => i.UserId).Contains(sentByUserId))
            {
                throw new ArgumentException("User is not part of the converastion");
            }

            //add message
            this.Messages.Add(new Message
            {
                Id = Guid.NewGuid(),
                SentByUserId = sentByUserId,
                SentOn = sentOn,
                Text = text
            });

            //reactivate interlocutor conversations
            this.ConversationOptions.ForEach(option =>
            {
                option.IsDeleted = false;
            });

            //set HasNewMessages for interlocutor
            this.ConversationOptions
                .Where(c => c.UserId != sentByUserId)
                .First()
                .HasNewMessages = true;
        }

        public void MarkConversationAsRead(Guid userId)
        {
            this.ConversationOptions
                .Where(c => c.UserId == userId)
                .First()
                .HasNewMessages = false;
        }

        public void Delete(Guid userId)
        {
            if (!this.ConversationOptions.Select(i => i.UserId).Contains(userId))
            {
                throw new ArgumentException("Only the creator is allowed to delete message");
            }

            foreach (var i in this.ConversationOptions.Where(i => i.UserId == userId))
            {
                i.IsDeleted = true;
            }
        }
    }
}
