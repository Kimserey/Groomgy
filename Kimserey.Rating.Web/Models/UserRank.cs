using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public class UserValue
    {
        public Guid UserId { get; set; }
        public int Value { get; set; }
    }
}
