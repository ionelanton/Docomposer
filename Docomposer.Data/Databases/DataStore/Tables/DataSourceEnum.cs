using System;

namespace Docomposer.Data.Databases.DataStore.Tables
{
    public enum DataSourceEnum
    {
        Excel = 1,
        Csv = 10, 
        Sqlite = 20,
        Postgresql = 30,
        MySql = 40,
        SqlServer = 50,
        WebService = 100 // json, xml
    }
    
    public static class DataSourceEnumExtensions {
        public static string ToFileExtension(this DataSourceEnum type)
        {
            var extension = type switch
            {
                DataSourceEnum.Excel => "xlsx",
                DataSourceEnum.Csv => "csv",
                DataSourceEnum.Sqlite => "db",
                _ => ""
            };
            
            if (extension == "") throw new ArgumentException($"Not configured data source type extension: {type}");
            
            return extension;
        }

        public static string ToString(this DataSourceEnum type)
        {
            var strType = type switch
            {
                DataSourceEnum.Excel => "Excel",
                DataSourceEnum.Csv => "CSV",
                DataSourceEnum.Sqlite => "Sqlite",
                _ => ""
            };
            
            if (strType == "") throw new ArgumentException($"Not configured data source type description: {type}");
            
            return strType;
        }
    }
}