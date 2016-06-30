using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class MessageEntity : Entity
    {
        public string Text { get; set; }
        public DateTime SentOn { get; set; }
        public bool IsDeleted { get; set; }

        public Guid SentByUserId { get; set; }
        public virtual UserEntity SentByUser { get; set; }

        public Guid ConversationId { get; set; }
        public virtual ConversationEntity Conversation { get; set; }
    }
}
