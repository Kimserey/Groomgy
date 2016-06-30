using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class VoteEntity : Entity
    {
        public int Rate { get; set; }
        public DateTime RatedOn { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("RatedByUser")]
        public Guid RatedByUserId { get; set; }
        public virtual UserEntity RatedByUser { get; set; }
        [ForeignKey("RatedOnUser")]
        public Guid RatedOnUserId { get; set; }
        public virtual UserEntity RatedOnUser { get; set; }
    }
}
