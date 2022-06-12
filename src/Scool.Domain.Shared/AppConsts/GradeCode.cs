using System.Collections.Generic;

namespace Scool.AppConsts
{
    public static class GradeCode
    {
        public const int Ten = 10;
        public const int Eleven = 11;
        public const int Twelve = 12;

        public static int GetGradeCodeOfClass(string name)
        {
            var grades = new List<string>
            {
                Ten.ToString(),
                Eleven.ToString(),
                Twelve.ToString(),
            };

            var code = grades.Find(x => name.StartsWith("Lớp") ? name.StartsWith($"Lớp {x}") : name.StartsWith(x));

            if (!string.IsNullOrEmpty(code))
            {
                return int.Parse(code);
            }

            return 0;
        }
    }
}
