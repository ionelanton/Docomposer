using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Core.Domain.DataSourceConfig;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Utils;
using LinqToDB;
using Newtonsoft.Json;

namespace Docomposer.Core.Api
{
    public static class DataSources
    {
        public static DataSource GetDataSourceById(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                var tableDataSource = db.DataSource.FirstOrDefault(s => s.Id == id);
                if (tableDataSource != null)
                {
                    return new DataSource
                    {
                        Id = tableDataSource.Id,
                        Name = tableDataSource.Name,
                        ProjectId = tableDataSource.ProjectId,
                        Configuration = tableDataSource.Configuration,
                        Type = tableDataSource.Type
                    };
                }

                return null;
            }
        }

        public static List<DataSource> GetDataSources()
        {
            using (var db = new DocReuseDataConnection())
            {
                return db.DataSource.Select(dt => new DataSource
                {
                    Id = dt.Id,
                    Name = dt.Name,
                    ProjectId = dt.ProjectId,
                    Configuration = dt.Configuration,
                    Type = dt.Type
                }).ToList();
            }
        }

        public static List<DataSource> GetDataSourcesByProjectId(int projectId)
        {
            using (var db = new DocReuseDataConnection())
            {
                return db.DataSource
                    .Where(ds => ds.ProjectId == projectId)
                    .Select(dt => new DataSource
                    {
                        Id = dt.Id,
                        Name = dt.Name,
                        ProjectId = dt.ProjectId,
                        Configuration = dt.Configuration,
                        Type = dt.Type
                    }).ToList();
            }
        }

        public static int AddDataSource(DataSource dataSource)
        {
            using (var db = new DocReuseDataConnection())
            {
                var dataSources = (from ds in db.DataSource
                    where ds.ProjectId == dataSource.ProjectId &&
                          ds.Name.Trim().ToLower() == dataSource.Name.Trim().ToLower()
                    select ds).ToList();
                if (dataSources.Count == 0)
                {
                    return db.InsertWithInt32Identity(dataSource);
                }

                throw new ArgumentException($"Data source {dataSource.Name} already in database");
            }
        }

        public static int UpdateDataSourceWithFile(DataSourceWithFile dataSourceWithFile)
        {
            var newDataSource = dataSourceWithFile.DataSource;
            var type = newDataSource.Type;

            using (var db = new DocReuseDataConnection())
            {
                var dataSources = (from ds in db.DataSource
                    where ds.Name == newDataSource.Name && ds.ProjectId == newDataSource.ProjectId
                    select ds).ToList();

                var id = 0;
                if (dataSources.Count == 1)
                {
                    var oldDataSource = dataSources.First();
                    switch (type)
                    {
                        case DataSourceEnum.Excel:
                        case DataSourceEnum.Csv:
                        case DataSourceEnum.Sqlite:
                            _updateDataSourceFileContent(newDataSource, oldDataSource, type,
                                dataSourceWithFile.FileContent);
                            break;
                        case DataSourceEnum.Postgresql:
                            break;
                        case DataSourceEnum.WebService:
                            break;
                        case DataSourceEnum.MySql:
                        case DataSourceEnum.SqlServer:
                        default:
                            break;
                    }

                    id = db.DataSource.Where(ds => ds.Id == newDataSource.Id).Update(ds => new TableDataSource
                    {
                        Type = newDataSource.Type,
                        Configuration = newDataSource.Configuration
                    });
                }
                else
                {
                    throw new ArgumentException($"Data source {newDataSource.Name} not found in database");
                }

                return id;
            }
        }

        public static int UpdateDataSourceById(DataSource dataSource)
        {
            using (var db = new DocReuseDataConnection())
            {
                var oldDataSource = (from ds in db.DataSource
                    where ds.Id == dataSource.Id
                    select ds).FirstOrDefault();

                var id = 0;
                if (oldDataSource != null)
                {
                    var type = oldDataSource.Type;

                    switch (type)
                    {
                        case DataSourceEnum.Excel:
                        case DataSourceEnum.Csv:
                        case DataSourceEnum.Sqlite:
                            _updateDataSourceFileName(dataSource, oldDataSource);
                            break;
                        case DataSourceEnum.Postgresql:
                            break;
                        case DataSourceEnum.WebService:
                            break;
                        case DataSourceEnum.MySql:
                        case DataSourceEnum.SqlServer:
                        default:
                            break;
                    }

                    id = db.DataSource.Where(ds => ds.Id == dataSource.Id).Update(ds => new TableDataSource
                    {
                        Name = dataSource.Name,
                        Configuration = dataSource.Configuration
                    });
                }
                else
                {
                    throw new ArgumentException($"Data source {dataSource.Name} not found in database");
                }

                return id;
            }
        }

        public static void DeleteDataSource(int id)
        {
            using var db = new DocReuseDataConnection();
            var dataSource = (from ds in db.DataSource
                where ds.Id == id
                select ds).FirstOrDefault();

            if (dataSource == null) return;

            var type = dataSource.Type;

            if (type > 0)
            {
                var config = JsonConvert.DeserializeObject<DataSourceExcelConfig>(dataSource.Configuration);
                switch (type)
                {
                    case DataSourceEnum.Excel:
                    case DataSourceEnum.Csv:
                    case DataSourceEnum.Sqlite:
                        if (config.FileName != null)
                        {
                            var file = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                                dataSource.ProjectId.ToString(),
                                ((int)type).ToString(),
                                config.FileName);

                            if (File.Exists(file))
                            {
                                File.Delete(file);
                            }
                        }

                        break;
                    case DataSourceEnum.Postgresql:
                        break;
                    case DataSourceEnum.WebService:
                        break;
                    case DataSourceEnum.MySql:
                        break;
                    case DataSourceEnum.SqlServer:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknkown data source type: {type.ToString()}");
                }
            }

            db.DataSource.Where(t => t.Id == id).Delete();
        }

        private static void _updateDataSourceFileName(TableDataSource dataSource, TableDataSource oldDataSource)
        {
            var newConfig = JsonConvert.DeserializeObject<DataSourceExcelConfig>(oldDataSource.Configuration);
            newConfig.FileName = dataSource.Name + ".xlsx";
            dataSource.Configuration = newConfig.Configuration();

            var newFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                dataSource.ProjectId.ToString(),
                ((int)DataSourceEnum.Excel).ToString(),
                newConfig.FileName);

            var oldConfig =
                JsonConvert.DeserializeObject<DataSourceExcelConfig>(oldDataSource.Configuration);

            if (oldConfig.FileName != null)
            {
                var oldFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                    dataSource.ProjectId.ToString(),
                    ((int)DataSourceEnum.Excel).ToString(),
                    oldConfig.FileName);

                var newDirectory = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                    dataSource.ProjectId.ToString(),
                    ((int)DataSourceEnum.Excel).ToString());

                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }

                if (File.Exists(oldFile))
                {
                    File.Move(oldFile, newFile, true);
                }
            }
        }

        private static void _updateDataSourceFileContent(TableDataSource newDataSource, TableDataSource oldDataSource,
            DataSourceEnum type, byte[] fileContent)
        {
            var newConfig = JsonConvert.DeserializeObject<DataSourceExcelConfig>(newDataSource.Configuration);

            newConfig.FileName = $"{newDataSource.Name}.{type.ToFileExtension()}";
            newDataSource.Configuration = newConfig.Configuration();

            var newFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                newDataSource.ProjectId.ToString(),
                ((int)type).ToString(),
                newConfig.FileName);

            var oldConfig =
                JsonConvert.DeserializeObject<DataSourceExcelConfig>(oldDataSource.Configuration);

            if (oldConfig.FileName != null)
            {
                var oldFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                    oldDataSource.ProjectId.ToString(),
                    ((int)type).ToString(),
                    oldConfig.FileName);

                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }
            }

            var newDirectory = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                newDataSource.ProjectId.ToString(),
                ((int)type).ToString());

            if (!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }

            File.WriteAllBytes(newFile, fileContent);
        }
    }
}