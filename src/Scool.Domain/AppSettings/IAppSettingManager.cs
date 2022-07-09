using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scool.AppSettings
{
    public interface IAppSettingManager
    {
        Task<string> GetValueAsync(string typeCode);
        Task<Dictionary<string, string>> GetReportSettingValuesAsync();
    }
}
