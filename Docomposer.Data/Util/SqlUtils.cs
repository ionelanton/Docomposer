using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Utils;
using Microsoft.Data.Sqlite;
using NPOI.SS.UserModel;

namespace Docomposer.Data.Util
{
    public static class SqlUtils
    {
        public static List<SqlParam> GetSqlParametersFromStatement(string statement)
        {
            var result = new List<SqlParam>();

            //const string pattern = @"{@(\w+)}";
            const string pattern = @"@(\w+)";

            var match = Regex.Matches(statement, pattern);

            foreach (Match m in match)
            {
                if (m.Groups.Count != 2) continue;

                result.Add(new SqlParam { Name = m.Groups[0].Value[1..], Value = "" });
            }

            return result;
        }

        public static DbCommand CreateDbCommandWithParameters(this DbConnection connection,
            DataSourceEnum dataSourceType, string sqlStatement,
            List<SqlParam> sqlParams)
        {
            DbCommand cmd = null;
            switch (dataSourceType)
            {
                case DataSourceEnum.Sqlite:
                    cmd = connection.CreateCommand();
                    cmd.CommandText = sqlStatement;
                    foreach (var param in sqlParams)
                    {
                        ((SqliteCommand)cmd).Parameters.AddWithValue("@" + param.Name, param.Value);
                    }

                    break;
            }

            return cmd;
        }

        public static SqliteConnection LoadExcelWorkbookIntoSqlite(IWorkbook workbook)
        {
            var connection = new SqliteConnection(ThisApp.SqliteMemoryConnectionString());
            connection.Open();

            using var transaction = connection.BeginTransaction();
            for (var i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                var columnNames = new List<string>();
                var parameterNames = new List<string>();
                var columnsNumber = 0;

                for (var row = 0; row <= sheet.LastRowNum;)
                {
                    columnsNumber = sheet.GetRow(row).Cells.Count;
                    for (var j = 0; j < columnsNumber; j++)
                    {
                        var name = sheet.GetRow(row).GetCell(j).StringCellValue;
                        columnNames.Add(name);
                        parameterNames.Add($"@{name}");
                    }

                    break;
                }

                var sqlColumns = $"'{string.Join("','", columnNames)}'";
                var sqlCreateTable = $"CREATE TABLE '{sheet.SheetName}' ({sqlColumns});";
                var sqlParameters = string.Join(",", parameterNames);

                var cmd = connection.CreateCommand();
                cmd.CommandText = sqlCreateTable;
                cmd.ExecuteNonQuery();
                cmd.CommandText =
                    $"INSERT INTO '{sheet.SheetName}' ({sqlColumns}) VALUES ({sqlParameters.Replace(" ", string.Empty)})";

                for (var row = 1; row <= sheet.LastRowNum; row++)
                {
                    cmd.Parameters.Clear();
                    for (var j = 0; j < columnsNumber; j++)
                    {
                        var cell = sheet.GetRow(row).GetCell(j);
                        var value = "";

                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            value = cell.StringCellValue;
                        }

                        cmd.Parameters.AddWithValue($"@{columnNames[j]}", value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }

            transaction.Commit();

            return connection;
        }

        public static SqliteConnection LoadDataTableIntoSqlite(DataTable dataTable)
        {
            var connection = new SqliteConnection(ThisApp.SqliteMemoryConnectionString());
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var columnNames = (from DataColumn column in dataTable.Columns select column.ColumnName).ToList();
            var paramNames = (from DataColumn column in dataTable.Columns select $"@{column.ColumnName}").ToList();

            var sqlColumns = $"'{string.Join("','", columnNames)}'";
            var sqlParameters = string.Join(",", paramNames);
            var sqlCreateTable = $"CREATE TABLE '{dataTable.TableName}' ({sqlColumns});";

            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlCreateTable;
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO '{dataTable.TableName}' ({sqlColumns}) VALUES ({sqlParameters})";

            foreach (DataRow row in dataTable.Rows)
            {
                cmd.Parameters.Clear();
                foreach (var columnName in columnNames)
                {
                    cmd.Parameters.AddWithValue($"@{columnName}", row[columnName].ToString());
                }

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();

            return connection;
        }

        public static List<string> GetTableNamesFromSqliteConnection(SqliteConnection connection)
        {
            var dataTable = new DataTable();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM sqlite_master WHERE type = 'table'";
            dataTable.Load(cmd.ExecuteReader());

            return dataTable.AsEnumerable().Select(r => r["tbl_name"].ToString()).ToList();
        }

        public static List<string> GetColumnNamesForTableFromSqliteConnection(SqliteConnection connection,
            string tableName)
        {
            var columnNames = new List<string>();

            var cmd = connection.CreateCommand();

            cmd.CommandText = $"SELECT * FROM '{tableName}' WHERE 0 = 1";

            var dataReader = cmd.ExecuteReader();
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                columnNames.Add(dataReader.GetName(i));
            }

            return columnNames;
        }

        public static bool HasSquareBracketsParamValue(string sqlParam)
        {
            const string pattern = @"^\s*\[[\s\w]+\s*\]\s*\.\s*\[[[\s\w]+\]\s*";
            
            var match = Regex.Match(sqlParam, pattern, RegexOptions.IgnoreCase);
            return !match.Success;
        }

        public static string SanitizeSquareBracketsParamValues(string sqlString)
        {
            var errorMessage = $"Invalid parameter format: {sqlString}. Expected: [x].[y]";

            if (HasSquareBracketsParamValue(sqlString))
            {
                return sqlString;
            }

            var startBrackets = new List<int>();
            var endBrackets = new List<int>();
            var parts = new List<string>();

            try
            {
                for (var i = sqlString.IndexOf('['); i > -1; i = sqlString.IndexOf('[', i + 1))
                {
                    startBrackets.Add(i);
                }

                for (var i = sqlString.IndexOf(']'); i > -1; i = sqlString.IndexOf(']', i + 1))
                {
                    endBrackets.Add(i);
                }

                for (var i = 0; i < startBrackets.Count; i++)
                {
                    parts.Add(
                        $"[{sqlString.Substring(startBrackets[i] + 1, endBrackets[i] - startBrackets[i] - 1).Trim()}]");
                }
            }
            catch (Exception ex)
            {
                //todo: log exception ex
                throw new ArgumentException(errorMessage);
            }

            return string.Join(".", parts);
        }
    }
}