using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IOnlineUserService
    {
        void SetAsOnline(Guid userId, string signalRConnectionId);
        void SetAsOffline(Guid userId);
        IQueryable<OnlineUserEntity> GetOnlineUsers();
    }
}
