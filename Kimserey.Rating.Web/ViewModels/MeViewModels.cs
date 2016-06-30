using Kimserey.Rating.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.ViewModels
{
    public class MeCreateViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }

    public class MeProfileViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public Gender Interest { get; set; }
    }
}