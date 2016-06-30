using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IUserService : IDisposable
    {
        Task CreateUser(Guid userId, string email, string name = null);
        Task UpdateGender(Guid userId, Gender gender);
        Task UpdateProfile(Guid userId, string name, string description, string location, DateTime birthday, Gender gender, Gender interest);
        Task SetProfilePhotoUrl(Guid userId, string photoUrl);
        Task AddAlbumPhotoUrl(Guid userId, string photoUrl);
        Task RemovePhotosFromAlbum(Guid userId, List<string> photoUrl);
        Task StopShowingWelcome(Guid userId);
        Task IncrementViewCount(Guid userId);
        Task EnableEmailNotification(Guid userId, bool isEnabled);
        IQueryable<UserEntity> QueryUser();
    }

}
