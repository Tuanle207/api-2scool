using Scool.AppConsts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class RegulationDataImportDto
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Point { get; set; }
        public string Type { get; set; }
        public string CriteriaName { get; set; }

        public static string GetRegulationType(string excelValue)
        {
            if (excelValue == "Lớp")
            {
                return RegulationType.Class;
            }
            else if (excelValue == "Học sinh")
            {
                return RegulationType.Student;
            }
            return string.Empty;
        }
    }
}
