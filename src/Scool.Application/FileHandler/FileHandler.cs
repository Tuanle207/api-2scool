using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Scool.Application.FileHandler
{
    public class FileHandler : IScopedDependency
    {
        private readonly IWebHostEnvironment _env;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IGuidGenerator _guidGenerator;

        public FileHandler(
            IWebHostEnvironment env,
            ICurrentUser currentUser,
            IHttpContextAccessor httpContext,
            IGuidGenerator guidGenerator)
        {
            _env = env;
            _currentUser = currentUser;
            _httpContext = httpContext;
            _guidGenerator = guidGenerator;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string groupName = "photo")
        {
            var basePath = _env.WebRootPath;
            var fileName = $"{_guidGenerator.Create()}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}{Path.GetExtension(file.FileName)}";
            if (file != null)
            {
                var path = Path.Combine(basePath, groupName, fileName);
                using (var stream = File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var photoUrl = $"{_httpContext.HttpContext.Request.Host.Value}/photo/{fileName}";
            return photoUrl;
        }

        public void RemoveFile(string path, string groupName = "photo")
        {
            var basePath = _env.WebRootPath;
            var decodeUrl = path.Split('/');
            if (decodeUrl.Length == 3)
            {
                var fileName = decodeUrl[2];
                var filePath = Path.Combine(basePath, groupName, fileName);
                File.Delete(filePath);
            }

        }
    }
}
