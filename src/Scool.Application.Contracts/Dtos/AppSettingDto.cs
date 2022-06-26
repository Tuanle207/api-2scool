using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class AppSettingDto : EntityDto<Guid>
    {
        public string TypeCode { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
