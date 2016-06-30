using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public class OnlineUserService : IOnlineUserService
    {
        public void SetAsOnline(Guid userId, string signalRConnectionId)
        {
            try
            {
                using (var db = new PeopleDbContext())
                {
                    var user = db.OnlineUserEntities.Find(userId);
                    if (user != null)
                    {
                        user.SignalRConnectionId = signalRConnectionId;
                    }
                    else
                    {
                        db.OnlineUserEntities.Add(new OnlineUserEntity
                        {
                            Id = userId,
                            SignalRConnectionId = signalRConnectionId
                        });
                    }
                    db.SaveChanges();
                }
            }
            catch
            { }
        }

        public void SetAsOffline(Guid userId)
        {
            try
            {
                using (var db = new PeopleDbContext())
                {
                    var user = db.OnlineUserEntities.Find(userId);
                    if (user == null) return;
                    db.OnlineUserEntities.Remove(user);

                    db.SaveChanges();
                }
            }
            catch
            { }
        }

        public IQueryable<OnlineUserEntity> GetOnlineUsers()
        {
            return new PeopleDbContext().OnlineUserEntities.AsQueryable();
        }
    }
}
