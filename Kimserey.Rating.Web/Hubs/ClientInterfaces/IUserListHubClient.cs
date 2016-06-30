using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Hubs.ClientInterfaces
{
    public interface IUserListHubClient
    {
        void UserConnected(Guid userId);
        void UserDisconnected(Guid userId);
    }
}
