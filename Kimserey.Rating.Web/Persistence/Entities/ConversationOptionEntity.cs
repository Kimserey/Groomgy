using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class ConversationOptionEntity : Entity
    {
        public bool IsDeleted { get; set; }
        public Guid UserId { get; set; }
        public bool HasNewMessages { get; set; }
    }
}
