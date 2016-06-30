using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using ImageResizer;
using System.Threading.Tasks;
using System.Drawing;

namespace Kimserey.Rating.Web.Services
{
    public class AzureFileStorageService : IFileStorageService
    {
        private CloudBlobContainer _container;

        public AzureFileStorageService()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(Consts.PhotoBlobContainerName);

            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
        }

        public async Task<string> SaveFile(Guid userId, string fileName, string extension, Stream fileStream)
        {
            var formats = new Dictionary<string, string>();
            formats.Add(Consts.ImageFormat.Small, "width=100&height=100&format=jpg&mode=crop");
            formats.Add(Consts.ImageFormat.Medium, "maxwidth=400&maxheight=200&format=jpg");
            formats.Add(Consts.ImageFormat.Large, "maxwidth=1920&maxheight=1080&format=jpg");

            using (fileStream)
            {
                await Task.WhenAll(formats
                    .Select(async format => await this.SaveFile(
                        string.Format("{0}{1}", fileName, extension),
                        string.Format("{0}/{1}", format.Key, userId),
                        fileStream,
                    format.Value)));
            }

            return string.Format("/{0}/{1}{2}", userId, fileName, extension);
        }

        public async Task<bool> DeleteFile(string filePath)
        {
            var deleteSmTask = this.DeleteFile(filePath, Consts.ImageFormat.Small);
            var deleteMdTask = this.DeleteFile(filePath, Consts.ImageFormat.Medium);
            var deleteLgTask = this.DeleteFile(filePath, Consts.ImageFormat.Large);
            return await deleteSmTask
                && await deleteMdTask
                && await deleteLgTask;
        }

        private async Task SaveFile(string fileName, string directory, Stream inputStream, string formatQuery)
        {
            CloudBlockBlob blockBlob = _container
                .GetDirectoryReference(directory)
                .GetBlockBlobReference(fileName);

            var image = Image.FromStream(inputStream);
            foreach (var prop in image.PropertyItems)
            {
                if (prop.Id == 0x0112)
                {
                    switch ((int)prop.Value[0])
                    {
                        case 1: break; //no rotation
                        case 2: image.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                        case 3: image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                        case 4: image.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                        case 5: image.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                        case 6: image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                        case 7: image.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                        case 8: image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                    }
                }
            }

            using (var memStream = new MemoryStream())
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                ImageBuilder.Current.Build(image, memStream, new ResizeSettings(formatQuery), false);

                memStream.Seek(0, SeekOrigin.Begin);
                await blockBlob.UploadFromStreamAsync(memStream);
            }
        }

        private Task<bool> DeleteFile(string fileName, string directory)
        {
            CloudBlockBlob blockBlob = _container
                .GetBlockBlobReference(string.Format("{0}{1}", directory, fileName));

            return blockBlob.DeleteIfExistsAsync();
        }
    }
}
