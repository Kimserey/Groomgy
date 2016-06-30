using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kimserey.Rating.Web.Hubs
{
    public class ProfileHub : Hub<IProfileHubClient>
    { }
}