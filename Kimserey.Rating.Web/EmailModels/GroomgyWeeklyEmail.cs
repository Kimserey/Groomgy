using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.EmailModels
{
    public class GroomgyWeeklyEmail
    {
        public string Email { get; set; }
        public string User { get; set; }
        public ConversationGroomgyWeeklyEmail[] Conversations { get; set; }
        public MemberSuggestedGroomgyWeeklyEmail[] Members { get; set; }
    }

    public class ConversationGroomgyWeeklyEmail
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Message { get; set; }
    }

    public class MemberSuggestedGroomgyWeeklyEmail
    {
        public string Image { get; set; }
        public string Url { get; set; }
    }
}
