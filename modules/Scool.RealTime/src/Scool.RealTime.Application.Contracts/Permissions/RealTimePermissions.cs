using Volo.Abp.Reflection;

namespace Scool.RealTime.Permissions
{
    public class RealTimePermissions
    {
        public const string GroupName = "RealTime";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(RealTimePermissions));
        }
    }
}