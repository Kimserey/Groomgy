using Kimserey.Rating.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class UserEntity : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public Gender Gender { get; set; }
        public Gender Interest { get; set; }
        public string Location { get; set; }
        //Json serialized list of string
        public string AlbumPhotoUrls { get; set; }
        public bool ShowWelcome { get; set; }
        public int CountProfileView { get; set; }
        public bool EmailNotificationEnabled { get; set; }
        public bool IsDeactivated { get; set; }

        public virtual ICollection<VoteEntity> SentVotes { get; set; }
        public virtual ICollection<VoteEntity> ReceivedVotes { get; set; }
        public virtual ICollection<ConversationEntity> Conversations { get; set; }
    }
}
