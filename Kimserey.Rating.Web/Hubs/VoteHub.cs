using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Hubs
{
    public class VoteHub: Hub<IVoteHubClient>
    { }
}
