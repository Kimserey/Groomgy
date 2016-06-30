using Kimserey.Rating.Web.Dto;
using Kimserey.Rating.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Hubs.ClientInterfaces
{
    public interface IProfileHubClient
    {
        void ProfileUpdated(Guid userId, string name, string description, string location, DateTime birthday, Gender gender);
        void ProfilePhotoChanged(Guid userId, PhotoDto photo);
        void GalleryPhotoUploaded(Guid userId, PhotoDto photo);
    }
}
