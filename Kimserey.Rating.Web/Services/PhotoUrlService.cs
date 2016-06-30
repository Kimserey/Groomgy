using Kimserey.Rating.Web.Dto;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public static class PhotoUrlService
    {
        private static string _containerUrl;

        static PhotoUrlService()
        {
            if (!string.IsNullOrEmpty(Consts.CdnUrl))
            {
                _containerUrl = Consts.CdnUrl + Consts.PhotoBlobContainerName;
            }
            else
            {
                _containerUrl = CloudStorageAccount
                        .Parse(ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString)
                        .CreateCloudBlobClient()
                        .GetContainerReference(Consts.PhotoBlobContainerName)
                        .Uri
                        .ToString();
            }
        }

        public static PhotoDto GetPhotoDto(string relativeUrl)
        {
            if (relativeUrl == null)
            {
                return new PhotoDto
                {
                    Small = "/Images/default_small_profile.png",
                    Medium = "/Images/default_profile.png",
                    Large = "/Images/default_profile.png"
                };
            }

            return new PhotoDto
            {
                Small = PhotoUrlService.GetUrl(Consts.ImageFormat.Small, relativeUrl),
                Medium = PhotoUrlService.GetUrl(Consts.ImageFormat.Medium, relativeUrl),
                Large = PhotoUrlService.GetUrl(Consts.ImageFormat.Large, relativeUrl),
                Relative = relativeUrl
            };
        }
        
        private static string GetUrl(string size, string relativeUrl)
        {
            return string.Format("{0}/{1}{2}", _containerUrl, size, relativeUrl);
        }
    }
}
