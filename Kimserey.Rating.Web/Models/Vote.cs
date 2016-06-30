using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public class Vote
    {
        public Guid Id { get; set; }
        public int Rate { get; set; }
        public Guid RatedByUserId { get; set; }
        public Guid RatedOnUserId { get; set; }
        public DateTime RatedOn { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }

        public Vote() { }

        public Vote(Guid id, int rate, string comment, Guid ratedByUserId, Guid ratedOnUserId, DateTime ratedOn)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("id cannot be empty guid");  
            }

            if (rate < 0 || rate > 5)
            {
                throw new ArgumentException("Rating cannot be superior to 5 and inferior to 0");
            }

            if (ratedOn < Consts.MinDate)
            {
                throw new ArgumentException("RatedOn date is invalid");
            }

            if (ratedByUserId == ratedOnUserId)
            {
                throw new ArgumentException("ratedByUserId cannot be the same as ratedOnUserId"); 
            }

            if (comment == null)
            {
                comment = string.Empty;
            }

            this.Id = id;
            this.Rate = rate;
            this.RatedByUserId = ratedByUserId;
            this.RatedOnUserId = ratedOnUserId;
            this.RatedOn = ratedOn;
            this.Comment = comment;
        }
    }
}
