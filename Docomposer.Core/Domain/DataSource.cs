using System;
using System.IO;
using Docomposer.Data.Databases.DataStore.Tables;

namespace Docomposer.Core.Domain
{
    public class DataSource : TableDataSource
    {
    }

    public class DataSourceWithFile
    {
        public DataSource DataSource { get; set; }
        public byte[] FileContent { get; set; }
    }
}