using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docomposer.Data.Util;
using Docomposer.Utils;
using Newtonsoft.Json;

namespace Docomposer.Core.Domain
{
    public class DataQueryModel : DataQuery
    {
        public DataTable DataTable { get; set; }

        public string Active { get; set; }

        public DataQueryModel Clone()
        {
            return new DataQueryModel
            {
                SqlParameters = SqlParameters.DeepCopy(),
                DataTable = DataTable.Copy(),
                Description = Description,
                Active = Active,
                Name = Name,
                Parameters = Parameters,
                DataSourceId = DataSourceId,
                Statement = Statement,
                Id = Id
            };
        }
    }
}