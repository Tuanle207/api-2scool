using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Scool.FileHandler
{
    public class FileHandler : IFileHandler, IScopedDependency
    {
        private static string BASE_PATH = null;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly IGuidGenerator _guidGenerator;

        public FileHandler(
            IConfiguration config,
            IWebHostEnvironment env,
            ICurrentUser currentUser,
            IHttpContextAccessor httpContext,
            IGuidGenerator guidGenerator)
        {
            _config = config;
            _env = env;
            _guidGenerator = guidGenerator;
        }

        public string GetBasePath()
        {
            if (string.IsNullOrEmpty(BASE_PATH))
            {
                string basePathFromConfig = _config["FileUploadBasePath"];
                BASE_PATH = string.IsNullOrEmpty(basePathFromConfig) ? _env.WebRootPath : basePathFromConfig;
            }
            return BASE_PATH;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string groupName = "photo")
        {
            var basePath = GetBasePath();
            var fileName = $"{_guidGenerator.Create()}-{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}{Path.GetExtension(file.FileName)}";
            if (file != null)
            {
                var path = Path.Combine(basePath, groupName, fileName);
                using (var stream = File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var photoUrl = $"/{groupName}/{fileName}";
            return photoUrl;
        }

        public void RemoveFile(string path, string groupName = "photo")
        {
            var basePath = GetBasePath();
            var decodeUrl = path.Split('/');
            if (decodeUrl.Length == 2)
            {
                var fileName = decodeUrl[2];
                var filePath = Path.Combine(basePath, groupName, fileName);
                File.Delete(filePath);
            }
        }
    }
}
