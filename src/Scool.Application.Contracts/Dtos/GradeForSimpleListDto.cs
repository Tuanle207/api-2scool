using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class GradeForSimpleListDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }
    }
}