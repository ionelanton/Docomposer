using System.Collections.Generic;
using System.Linq;

namespace Docomposer.Data.Util
{
    public class SqlParam
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public SqlParam Copy()
        {
            return new SqlParam
            {
                Name = Name,
                Value = Value
            };
        }

        public static List<SqlParam> CopyList(List<SqlParam> sqlParams)
        {
            return sqlParams.Select(p => p.Copy()).ToList();
        }
    }
}