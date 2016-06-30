using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class ConversationEntity : Entity
    {
        public virtual ICollection<MessageEntity> Messages { get; set; }
        public virtual ICollection<ConversationOptionEntity> ConversationOptions { get; set; }
        public virtual ICollection<UserEntity> Users { get; set; }
    }
}