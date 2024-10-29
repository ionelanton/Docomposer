using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docomposer.Core.Api;
using Docomposer.Data.Util;

namespace Docomposer.Core.Domain;

public static class DataQueryModelExtensions
{
    public static List<DataQueryModel> UpdateDataTablesFromQueries(this IEnumerable<DataQueryModel> queries, int limitRowCount = 0)
    {
        var copiedDataModelQueries = queries.Select(q => q.Clone()).ToList();
        
        DataQueryModel previousQuery = null;

        foreach (var query in copiedDataModelQueries)
        {
            if (previousQuery != null)
            {
                var foundSqlParam = query.SqlParameters.FirstOrDefault(p => SqlUtils.SanitizeSquareBracketsParamValues(p.Value).Contains($"[{previousQuery.DataTable.TableName}]"));

                if (foundSqlParam != null)
                {
                    foundSqlParam.Value = SqlUtils.SanitizeSquareBracketsParamValues(foundSqlParam.Value);
                    var columnName = foundSqlParam.Value.Split('.')[1].Replace("[", "").Replace("]", "");

                    var value = previousQuery.DataTable.Rows.OfType<DataRow>().Select(r => r[columnName].ToString()).First();

                    foundSqlParam.Value = value;
                }
            }

            query.SyncParameters();
            query.DataTable = limitRowCount > 0 
                ? DataQueries.DataTableFromQueryDataSource(query).AsEnumerable().Take(limitRowCount).CopyToDataTable() 
                : DataQueries.DataTableFromQueryDataSource(query).AsEnumerable().CopyToDataTable();
            query.DataTable.TableName = query.Name;
            previousQuery = query;
        }

        return copiedDataModelQueries;
    }
}