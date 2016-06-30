using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFile(Guid userId, string fileName, string extension, Stream fileStream);
        Task<bool> DeleteFile(string filePath);
    }
}
