using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class TeacherForSimpleListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
