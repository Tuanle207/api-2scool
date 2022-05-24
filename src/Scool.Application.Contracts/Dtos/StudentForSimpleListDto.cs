using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class StudentForSimpleListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
