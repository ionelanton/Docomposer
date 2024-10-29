using System.Collections.Generic;
using Docomposer.Core.Api;
using Docomposer.Data.Test.Database;
using Docomposer.Data.Util;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Docomposer.Core.Test.Api
{
    [TestFixture]
    public class TestDataQueries : TestDatabase
    {
        [Test]
        public void TestDataTableFromQueryDataSource()
        {
            var dataQuery = DataQueries.GetDataQueryById(1);

            dataQuery.Parameters = JsonConvert.SerializeObject(new List<SqlParam> { new() { Name = "CustomerId", Value = "1"} });

            DataQueries.UpdateDataQuery(dataQuery);

            var dataTable = DataQueries.DataTableFromQueryDataSource(dataQuery);

            Assert.That(dataTable.Columns.Count, Is.EqualTo(9));
            Assert.That(dataTable.Rows.Count, Is.EqualTo(1));
        }
    }
}