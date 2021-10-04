using Microsoft.AspNetCore.Http;
using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateLRReportDto
    {
        public Guid ClassId { get; set; }
        public IFormFile Photo { get; set; }
        public int AbsenceNo { get; set; }
        public int TotalPoint { get; set; }
}
}
