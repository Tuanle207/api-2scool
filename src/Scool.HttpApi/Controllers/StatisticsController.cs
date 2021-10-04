using Microsoft.AspNetCore.Mvc;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Controllers
{
    public class StatisticsController : ScoolController
    {
        private readonly IStatisticsAppService _statisticsAppService;

        public StatisticsController(IStatisticsAppService statisticsAppService)
        {
            _statisticsAppService = statisticsAppService;
        }

        [HttpGet("downloads/classes-faults-excel")]
        public async Task<IActionResult> GetClassesFaultsExcel([FromQuery]TimeFilterDto timeFilter)
        {
            var memoryStream = await _statisticsAppService.GetClassesFaultsExcel(timeFilter);
            memoryStream.Position = 0;
            var fileName = $"thong-ke-lop-vi-pham-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.xlsx";
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("downloads/dcp-ranking-excel")]
        public async Task<IActionResult> GetDcpRankingExcel([FromQuery] TimeFilterDto timeFilter)
        {
            var memoryStream = await _statisticsAppService.GetDcpRankingExcel(timeFilter);
            memoryStream.Position = 0;
            var fileName = $"bap-cao-xep-hang-ne-nep-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.xlsx";
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("downloads/common-faults-excel")]
        public async Task<IActionResult> GetCommonFaultsExcel([FromQuery] TimeFilterDto timeFilter)
        {
            var memoryStream = await _statisticsAppService.GetCommonFaultsExcel(timeFilter);
            memoryStream.Position = 0;
            var fileName = $"thong-ke-loi-vi-pham-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.xlsx";
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("downloads/students-with-most-faults-excel")]
        public async Task<IActionResult> GetStudentsWithMostFaultsExcel([FromQuery] TimeFilterDto timeFilter)
        {
            var memoryStream = await _statisticsAppService.GetStudentsWithMostFaultsExcel(timeFilter);
            memoryStream.Position = 0;
            var fileName = $"thong-ke-hoc-sinh-vi-pham-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.xlsx";
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
