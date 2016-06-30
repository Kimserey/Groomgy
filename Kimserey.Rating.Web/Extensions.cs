using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.AspNet.Identity
{
    public static class Extensions
    {
        public static Guid? GetUserIdGuid(this IIdentity identity)
        {
            if (!identity.IsAuthenticated)
            {
                return null;
            }

            return new Guid(identity.GetUserId());
        }
    }
}
