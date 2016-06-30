using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.ViewModels
{
    public class VoteViewModel
    {
        public Guid RatedOnUserId { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public DateTime VotedOn { get; set; }
    }
}
