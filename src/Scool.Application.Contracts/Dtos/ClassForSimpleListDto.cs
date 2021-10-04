using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class ClassForSimpleListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
