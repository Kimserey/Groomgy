using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Kimserey.Rating.Web.Services;

namespace Kimserey.Rating.Web.Hubs
{
    public class UserListHub : Hub<IUserListHubClient>
    {
        private IOnlineUserService _onlineUserService;

        public UserListHub(IOnlineUserService onlineUserService)
        {
            _onlineUserService = onlineUserService;
        }

        public override Task OnConnected()
        {
            Guid userId = GetAuthenticatedUserId();
            if (userId != Guid.Empty)
            {
                    _onlineUserService.SetAsOnline(userId, this.Context.ConnectionId);
                    this.Clients.All.UserConnected(userId);
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Guid userId = GetAuthenticatedUserId();
            if (userId != Guid.Empty)
            {
                _onlineUserService.SetAsOnline(userId, this.Context.ConnectionId);
                this.Clients.All.UserConnected(userId);
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Guid userId = GetAuthenticatedUserId();
            if (userId != Guid.Empty)
            {
                _onlineUserService.SetAsOffline(userId);
                this.Clients.All.UserDisconnected(userId);
            }
            return base.OnDisconnected(stopCalled);
        }

        private Guid GetAuthenticatedUserId()
        {
            if(this.Context.User.Identity.IsAuthenticated) {
                var id = this.Context.User.Identity.GetUserId();
                return new Guid(id);
            }
            return Guid.Empty;
        }
    }
}