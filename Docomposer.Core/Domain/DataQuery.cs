using System.Collections.Generic;
using System.Linq;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using Docomposer.Utils;
using Newtonsoft.Json;

namespace Docomposer.Core.Domain
{
    public class DataQuery : TableDataQuery
    {
        public List<SqlParam> SqlParameters { get; set; } = new();

        public void SyncParameters()
        {
            var sqlParamsFromStatement = SqlUtils.GetSqlParametersFromStatement(Statement);
            
            if (Parameters != null && Parameters.Trim().Length > 0 && SqlParameters.Count == 0)
            {
                SqlParameters = JsonConvert.DeserializeObject<List<SqlParam>>(Parameters);
            } 

            foreach (var sqlParamFromStatement in sqlParamsFromStatement)
            {
                var sqlParam = SqlParameters.FirstOrDefault(p => p.Name == sqlParamFromStatement.Name);

                if (sqlParam != null)
                {
                    sqlParamFromStatement.Value = sqlParam.Value;
                }
            }

            SqlParameters = sqlParamsFromStatement;
            Parameters = JsonConvert.SerializeObject(sqlParamsFromStatement);
        }
    }
}