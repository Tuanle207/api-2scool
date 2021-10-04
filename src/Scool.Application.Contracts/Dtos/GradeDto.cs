using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class GradeDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
