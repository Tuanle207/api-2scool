using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Scool.Application.FileHandler
{
    public interface IFileHandler
    {
        Task<string> SaveFileAsync(IFormFile file, string groupName = "photo");

        void RemoveFile(string path, string groupName = "photo");
    }
}