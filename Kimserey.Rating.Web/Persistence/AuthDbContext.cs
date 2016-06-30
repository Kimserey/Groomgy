using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext() : base("AuthDbContext")
        { }
    }
}
