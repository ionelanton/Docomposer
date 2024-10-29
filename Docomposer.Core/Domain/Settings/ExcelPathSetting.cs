using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util.Localization;
using Newtonsoft.Json;

namespace Docomposer.Core.Domain.Settings
{
    public class ExcelPathSetting : TableSetting, ISetting
    {
        public ExcelPathSetting GetSetting<ExcelPathSetting>()
        {
            return JsonConvert.DeserializeObject<ExcelPathSetting>(Value);
        }

        public void SetSetting<ExcelPathSetting>(ExcelPathSetting setting)
        {
            Key = nameof(Localization.Settings.ExcelPath);
            Value = JsonConvert.SerializeObject(this);
        }
    }
}