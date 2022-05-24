using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class GradeForSimpleListDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }
    }
}