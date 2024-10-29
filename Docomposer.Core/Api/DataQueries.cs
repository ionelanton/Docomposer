using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Docomposer.Core.Domain;
using Docomposer.Core.Domain.DataSourceConfig;
using Docomposer.Core.LiquidXml;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using Docomposer.Data.Util;
using Docomposer.Utils;
using DocumentFormat.OpenXml.Packaging;
using LinqToDB;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UtfUnknown;

namespace Docomposer.Core.Api
{
    public class DataQueries
    {
        public static DataSource GetDataSourceFromDataQuery(DataQuery query)
        {
            using var db = new DocReuseDataConnection();
            var tableDataSource = db.DataSource.FirstOrDefault(s => s.Id == query.DataSourceId);
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

        public static DataQuery GetDataQueryById(int id)
        {
            using var db = new DocReuseDataConnection();
            var tableDataQuery = db.DataQuery.FirstOrDefault(s => s.Id == id);
            if (tableDataQuery != null)
            {
                return new DataQuery
                {
                    Id = tableDataQuery.Id,
                    Name = tableDataQuery.Name,
                    Description = tableDataQuery.Description,
                    Statement = tableDataQuery.Statement,
                    DataSourceId = tableDataQuery.DataSourceId,
                    Parameters = tableDataQuery.Parameters
                };
            }

            return null;
        }

        public static IEnumerable<DataQuery> GetDataQueriesBySectionId(int sectionId)
        {
            using var db = new DocReuseDataConnection();

            var section = Sections.GetSectionById(sectionId);

            //todo: use IFileHandler
            var sectionPath = ThisApp.FileHandler().CombinePaths(ThisApp.DocReuseDocumentsPath(), section.ProjectId.ToString(), DirName.Sections, section.Name + ".docx");

            using (var wd = WordprocessingDocument.Open(sectionPath, false))
            {
                var queryNames = wd.GetQueryNamesFromDocument();
                
                var queriesInProject = (from q in db.DataQuery
                    from ds in db.DataSource.LeftJoin(qs => qs.Id == q.DataSourceId)
                    where ds.ProjectId == section.ProjectId
                    select new DataQuery
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Description = q.Description,
                        Statement = q.Statement,
                        DataSourceId = q.DataSourceId,
                        Parameters = q.Parameters
                    }).OrderBy(q => q.Id).ToList();

                return queriesInProject.Where(q => queryNames.Any(qn => qn == q.Name));
            }
        }

        public static IEnumerable<DataQuery> GetDataQueriesByDataSourceId(int dataSourceId)
        {
            using var db = new DocReuseDataConnection();
            return (from q in db.DataQuery
                where q.DataSourceId == dataSourceId
                select new DataQuery
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    Statement = q.Statement,
                    DataSourceId = q.DataSourceId,
                    Parameters = q.Parameters
                }).OrderBy(q => q.Id).ToList();
        }

        public static DataQuery GetDataQueryByDataSourceIdAndQueryName(int dataSourceId, string name)
        {
            using var db = new DocReuseDataConnection();
            return (from q in db.DataQuery
                where q.DataSourceId == dataSourceId && q.Name == name
                select new DataQuery
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    Statement = q.Statement,
                    DataSourceId = q.DataSourceId,
                    Parameters = q.Parameters
                }).OrderBy(q => q.Id).FirstOrDefault();
        }

        public static List<DataQuery> GetDataQueriesByDocumentId(int documentId)
        {
            var dataQueries = new List<DataQuery>();
            
            using (var db = new DocReuseDataConnection())
            {
                var sectionsInDocument = (from sd in db.SectionDocument
                    from s in db.Section.LeftJoin(s => sd.SectionId == s.Id)
                    where sd.DocumentId == documentId
                    select new Section
                    {
                        Id = s.Id,
                        ProjectId = s.ProjectId,
                        Name = s.Name
                    }).Distinct().ToList();

                foreach (var section in sectionsInDocument)
                {
                    var queriesInSection = GetDataQueriesBySectionId(section.Id);

                    foreach (var dataQuery in queriesInSection)
                    {
                        if (!dataQueries.Contains(dataQuery))
                        {
                            dataQueries.Add(dataQuery);
                        }
                    }    
                }
            }

            return dataQueries.ToList();
        }

        public static List<DataQuery> GetDataQueriesByCompositionId(int compositionId)
        {
            var dataQueries = new List<DataQuery>();
            
            using (var db = new DocReuseDataConnection())
            {
                var sectionsInComposition = (from s in db.Section
                    from sd in db.SectionDocument.LeftJoin(sd => s.Id == sd.SectionId)
                    from da in db.DocumentComposition.LeftJoin(da => da.DocumentId == sd.DocumentId)
                    where da.CompositionId == compositionId
                    select new Section
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ProjectId = s.ProjectId
                    }).Distinct().ToList();
                
                foreach (var section in sectionsInComposition)
                {
                    var queriesInSection = GetDataQueriesBySectionId(section.Id);

                    foreach (var dataQuery in queriesInSection)
                    {
                        if (!dataQueries.Contains(dataQuery))
                        {
                            dataQueries.Add(dataQuery);
                        }
                    }
                }
            }
            
            return dataQueries.ToList();
        }

        public static int UpdateDataQuery(DataQuery dataQuery)
        {
            using var db = new DocReuseDataConnection();

            var dataSource = db.DataSource.FirstOrDefault(ds => ds.Id == dataQuery.DataSourceId);
            
            if (dataSource == null) throw new ArgumentException($"Data query '{dataQuery.Name}' not linked to a data source");
            
            var dataSourcesInProject =
                (from ds in db.DataSource where ds.ProjectId == dataSource.ProjectId select ds.Id).ToList();

            var queriesInProject = (from q in db.DataQuery where dataSourcesInProject.Contains(q.DataSourceId) select q).ToList();

            if (!queriesInProject.Exists(q => string.Equals(q.Name.Trim(), dataQuery.Name.Trim()) && q.Id != dataQuery.Id))
            {
                return db.DataQuery.Where(q => q.Id == dataQuery.Id).Update(ds => new TableDataQuery
                {
                    Name = dataQuery.Name,
                    Description = dataQuery.Description,
                    Statement = dataQuery.Statement,
                    DataSourceId = dataQuery.DataSourceId,
                    Parameters = dataQuery.Parameters
                });
            }
            
            throw new ArgumentException($"A query named '{dataQuery.Name}' exists in this project. Choose another name.");
        }

        public static int DeleteDataQueryById(int id)
        {
            using var db = new DocReuseDataConnection();

            return db.DataQuery.Where(q => q.Id == id).Delete();
        }

        public static int AddDataQuery(DataQuery dataQuery)
        {
            using var db = new DocReuseDataConnection();

            var dataSource = db.DataSource.FirstOrDefault(ds => ds.Id == dataQuery.DataSourceId);

            if (dataSource == null) throw new ArgumentException($"Data query '{dataQuery.Name}' not linked to a data source");

            var dataSourcesInProject =
                (from ds in db.DataSource where ds.ProjectId == dataSource.ProjectId select ds.Id).ToList();

            var queriesInProject = (from q in db.DataQuery where dataSourcesInProject.Contains(q.DataSourceId) select q).ToList();
            
            if (!queriesInProject.Exists(q => string.Equals(q.Name.Trim(), dataQuery.Name.Trim())))
            {
                return db.InsertWithInt32Identity(dataQuery);
            }

            throw new ArgumentException($"A query named '{dataQuery.Name}' exists in this project. Choose another name.");
        }

        public static DataTable DataTableFromQueryDataSource(DataQuery query)
        {
            var dataSource = GetDataSourceFromDataQuery(query);

            if (dataSource == null)
            {
                return DataTableFromQueryWithoutDataSource(query);
            }

            switch (dataSource.Type)
            {
                case DataSourceEnum.Excel:
                    return _resultsFromExcelDataSource(dataSource, query);
                case DataSourceEnum.Csv:
                    return _resultsFromCsvDataSource(dataSource, query);
                case DataSourceEnum.Sqlite:
                    return _resultsFromSqliteDataSource(dataSource, query);
                case DataSourceEnum.Postgresql:
                    break;
                case DataSourceEnum.WebService:
                    break;
                case DataSourceEnum.MySql:
                case DataSourceEnum.SqlServer:
                
                default:
                    throw new ArgumentOutOfRangeException($"Unknown data source {dataSource.Type}");
            }

            return null;
        }

        private static DataTable _resultsFromExcelDataSource(DataSource dataSource, DataQuery query)
        {
            var dataSourceConfig =
                JsonConvert.DeserializeObject<DataSourceExcelConfig>(dataSource.Configuration);

            if (dataSourceConfig.FileName == null)
            {
                throw new ArgumentException($"Missing Excel file for {dataSource.Name} data source");
            }

            var excelFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                dataSource.ProjectId.ToString(),
                ((int)dataSource.Type).ToString(),
                dataSourceConfig.FileName);

            var fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IWorkbook book = new XSSFWorkbook(fs);

       
            var sqliteConnection = SqlUtils.LoadExcelWorkbookIntoSqlite(book);

            var dataTable = new DataTable();
            var sqlParams = JsonConvert.DeserializeObject<List<SqlParam>>(query.Parameters);
            var cmd2 = sqliteConnection.CreateDbCommandWithParameters(DataSourceEnum.Sqlite, query.Statement,
                sqlParams);

            dataTable.Load(cmd2.ExecuteReader());

            sqliteConnection.Close();

            return dataTable;
        }

        private static DataTable DataTableFromQueryWithoutDataSource(DataQuery query)
        {
            using var connection = new SqliteConnection(ThisApp.SqliteMemoryConnectionString());
            
            connection.Open();
            var dataTable = new DataTable();
            var cmd = connection.CreateDbCommandWithParameters(DataSourceEnum.Sqlite, query.Statement,
                JsonConvert.DeserializeObject<List<SqlParam>>(query.Parameters));
            dataTable.Load(cmd.ExecuteReader());
            connection.Close();

            return dataTable;
        }

        private static DataTable _resultsFromCsvDataSource(DataSource dataSource, DataQuery query)
        {
            var dataSourceConfig =
                JsonConvert.DeserializeObject<DataSourceCsvConfig>(dataSource.Configuration);

            if (dataSourceConfig.FileName == null)
            {
                throw new ArgumentException($"Missing CSV file for {dataSource.Name} data source");
            }

            var csvFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                dataSource.ProjectId.ToString(),
                ((int)dataSource.Type).ToString(),
                dataSourceConfig.FileName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            var csvFileEncoding = CharsetDetector.DetectFromFile(csvFile);
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLowerInvariant().Trim().Replace(" ", "_"),
                Encoding = csvFileEncoding.Detected.Encoding,
                DetectDelimiter = true,
            };
            
            using var reader = new StreamReader(csvFile, csvFileEncoding.Detected.Encoding);
            using var csv = new CsvReader(reader, config);
        
            using var dataReader = new CsvDataReader(csv);
            var dt = new DataTable();
            dt.TableName = dataSource.Name;
            dt.Load(dataReader);

            var sqliteConnection = SqlUtils.LoadDataTableIntoSqlite(dt);

            var dataTable = new DataTable();
            var sqlParams = JsonConvert.DeserializeObject<List<SqlParam>>(query.Parameters);
            var cmd2 = sqliteConnection.CreateDbCommandWithParameters(DataSourceEnum.Sqlite, query.Statement,
                sqlParams);

            dataTable.Load(cmd2.ExecuteReader());

            sqliteConnection.Close();

            return dataTable;
        }
        
        private static DataTable _resultsFromSqliteDataSource(DataSource dataSource, DataQuery query)
        {
            var dataSourceConfig =
                JsonConvert.DeserializeObject<DataSourceCsvConfig>(dataSource.Configuration);

            if (dataSourceConfig.FileName == null)
            {
                throw new ArgumentException($"Missing SQLite file for {dataSource.Name} data source");
            }

            var sqliteFile = Path.Combine(ThisApp.DocReuseDocumentsPath(), DirName.DataSources,
                dataSource.ProjectId.ToString(),
                ((int)dataSource.Type).ToString(),
                dataSourceConfig.FileName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
           
            var connection = new SqliteConnection($"Data Source={sqliteFile}");
            connection.Open();
           
            var dataTable = new DataTable();
            var sqlParams = JsonConvert.DeserializeObject<List<SqlParam>>(query.Parameters);
            var cmd = connection.CreateDbCommandWithParameters(DataSourceEnum.Sqlite, query.Statement,
                sqlParams);

            dataTable.Load(cmd.ExecuteReader());

            connection.Close();

            return dataTable;
        }
    }
}