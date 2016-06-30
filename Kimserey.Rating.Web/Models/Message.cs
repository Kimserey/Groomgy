using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid SentByUserId { get; set; }
        public DateTime SentOn { get; set; }
    }
}
