using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Docomposer.Core.Domain.DataSourceConfig
{
    public class DataSourceExcelConfig : IDataSourceConfig
    {
        [Required]
        public string FileName { get; set; }
        public string Configuration()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}