using AutoMapper;
using Kimserey.Rating.Web.Models;
using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Kimserey.Rating.Web.Services
{
    public class UserService : IUserService
    {
        private IRepository _repository;
        private IEmailService _emailService;

        static UserService()
        {
            Mapper.CreateMap<UserEntity, User>()
                .ForMember(ent => ent.AlbumPhotoUrls, x => x.Ignore())
                .AfterMap((ent, user) => user.AlbumPhotoUrls = JsonConvert.DeserializeObject<List<string>>(ent.AlbumPhotoUrls));
        }

        public UserService(IRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task CreateUser(Guid userId, string email, string name = null)
        {
            User user;
            if (name == null)
            {
                user = new User(userId, email);
            }
            else
            {
                user = new User(userId, email, name);
            }
            await _repository.SaveUser(user);
            await _emailService.SendNewUserEmail(user.Id, user.Email);
        }

        public Task UpdateGender(Guid userId, Gender gender)
        {
            return this.ModifyUser(userId, user => user.UpdateGender(gender));
        }

        public Task UpdateProfile(Guid userId, string name, string description, string location, DateTime birthday, Gender gender, Gender interest)
        {
            return this.ModifyUser(userId, user => user.UpdateProfile(name, description, location, birthday, gender, interest));
        }

        public Task SetProfilePhotoUrl(Guid userId, string photoUrl)
        {
            return this.ModifyUser(userId, user => user.SetProfilePhotoUrl(photoUrl));
        }

        public Task AddAlbumPhotoUrl(Guid userId, string photoUrl)
        {
            return this.ModifyUser(userId, user => user.AddAlbumPhotoUrl(photoUrl));
        }

        public Task RemovePhotosFromAlbum(Guid userId, List<string> photoUrl)
        {
            return this.ModifyUser(userId, user => photoUrl.ForEach(url => user.RemovePhotoFromAlbum(url)));
        }

        public Task StopShowingWelcome(Guid userId)
        {
            return this.ModifyUser(userId, user => user.StopShowingWelcome());
        }

        public Task EnableEmailNotification(Guid userId, bool isEnabled)
        {
            return this.ModifyUser(userId, user => user.EnableEmailNotification(isEnabled));
        }

        public Task IncrementViewCount(Guid userId)
        {
            return this.ModifyUser(userId, user => user.IncrementViewCount());
        }

        private async Task ModifyUser(Guid userId, Action<User> action)
        {
            User user = await this.GetUser(userId);

            if (user == null)
            {
                throw new ApplicationException(string.Format("User of id {0} does not exists", userId));
            }

            action(user);
            await _repository.SaveUser(user);
        }

        private async Task<User> GetUser(Guid userId)
        {
            UserEntity userEntity = await _repository.QueryUser().FirstOrDefaultAsync(u => u.Id == userId);

            if (userEntity == null)
            {
                throw new ApplicationException(string.Format("User of id {0} does not exists", userId));
            }

            User user = Mapper.Map<User>(userEntity);
            return user;
        }

        public IQueryable<UserEntity> QueryUser()
        {
            return _repository.QueryUser();
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
