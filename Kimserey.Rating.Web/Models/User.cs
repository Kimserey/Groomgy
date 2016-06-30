using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Models
{
    public enum Gender
    {
        NotSpecified,
        Woman,
        Man
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Gender Interest { get; set; }
        public string Location { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public List<string> AlbumPhotoUrls { get; set; }
        public bool ShowWelcome { get; set; }
        public int CountProfileView { get; set; }
        public bool EmailNotificationEnabled { get; set; }

        public User() { }

        public User(Guid id, string email, string name)
            : this(id, email)
        {
            this.Name = name;
        }

        public User(Guid id, string email)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty guid");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be empty string.");
            }


            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = this.GenerateGroomgyName();
            }

            this.Id = id;
            this.Email = email;
            this.AlbumPhotoUrls = new List<string>();
            this.Birthday = DateTime.Now;
            this.ShowWelcome = true;
            this.EmailNotificationEnabled = true;
        }

        public void UpdateGender(Gender gender)
        {
            this.Gender = gender;
        }

        public void UpdateProfile(string name, string description, string location, DateTime birthday, Gender gender, Gender interest)
        {
            if (birthday <= Consts.MinDate || birthday > DateTime.UtcNow || DateTime.UtcNow.Year - birthday.Year < 18)
            {
                throw new ArgumentException("Birthday date is invalid");
            }

            this.Name = name;
            this.Description = description;
            this.Birthday = birthday;
            this.Gender = gender;
            this.Location = location;
            this.ShowWelcome = false;
            this.Interest = interest;
        }

        public void SetProfilePhotoUrl(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                throw new ArgumentException("PhotoUrl cannot be null or empty");
            }

            this.ProfilePhotoUrl = photoUrl;
        }

        public void AddAlbumPhotoUrl(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                throw new ArgumentException("PhotoUrl cannot be null or empty");
            }

            if (this.AlbumPhotoUrls.Any(i => i == photoUrl))
                return;

            this.AlbumPhotoUrls.Add(photoUrl);
        }

        public void RemovePhotoFromAlbum(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                throw new ArgumentException("PhotoUrl cannot be null or empty");
            }

            if (!this.AlbumPhotoUrls.Any(i => i == photoUrl))
                return;

            this.AlbumPhotoUrls.Remove(photoUrl);
        }

        public void StopShowingWelcome()
        {
            this.ShowWelcome = false;
        }

        public void IncrementViewCount()
        {
            this.CountProfileView += 1;
        }

        public void EnableEmailNotification(bool isEnabled)
        {
            this.EmailNotificationEnabled = isEnabled;
        }

        private string GenerateGroomgyName()
        {
            return string.Format("Groomgy-user-{0}", new Random().Next(99999));
        }
    }
}
