using System.Collections.Generic;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Data.Util;

namespace Docomposer.Blazor.Data
{
    public static class Utils
    {
        public static SqlParam ParameterWithoutValue(List<DataQueryModel> queries)
        {
            var query = queries.FirstOrDefault(q => q.SqlParameters.Exists(p => p.Value.Trim() == ""));
            var param = query?.SqlParameters.FirstOrDefault(p => p.Value.Trim() == "");
            return param;
        }
        
        public static SqlParam ParameterWithoutValue(DataQueryModel query)
        {
            var param = query?.SqlParameters.FirstOrDefault(p => p.Value.Trim() == "");
            return param;
        }
    }
}