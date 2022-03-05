using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Scool.IApplicationServices
{
    public interface IDataImportAppService : IApplicationService
    {
        Task PostImportStudentsData(IFormFile file);

        Task PostImportTeachersData(IFormFile file);

        Task PostImportClassesData(IFormFile file);

        Task PostImportRegulationsData(IFormFile file);

    }
}
