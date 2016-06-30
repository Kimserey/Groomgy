using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.ViewModels
{
    public class MessageViewModel
    {
        public Guid ConversationId { get; set; }
        public string Text { get; set; }
        public DateTime SentOn { get; set; }
    }

    public class MessagePostedViewModel
    {
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime SentOn { get; set; }
    }

}
