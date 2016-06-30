using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kimserey.Rating.Web.Persistence.Entities
{
    public class OnlineUserEntity: Entity
    {
        public string SignalRConnectionId { get; set; }
    }
}