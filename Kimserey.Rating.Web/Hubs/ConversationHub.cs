using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Kimserey.Rating.Web.Hubs
{
    [Authorize]
    public class ConversationHub : Hub<IConversationHubClient>
    { }
}
