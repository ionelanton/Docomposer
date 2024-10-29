using System.IO;
using System.Linq;
using Docomposer.Data.Util;
using Docomposer.Utils;
using Microsoft.Data.Sqlite;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NUnit.Framework;

namespace Docomposer.Data.Test.Util;

[TestFixture]
public class TestExcel
{
    private SqliteConnection _connection;

    [OneTimeSetUp]
    public void SetUp()
    {
        var chinookExcelFile =
            Path.Combine(ThisApp.DocReuseDocumentsPath(), @"DataSources\3\1", "chinook.xlsx");

        var fs = new FileStream(chinookExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        IWorkbook book = new XSSFWorkbook(fs);

        _connection = SqlUtils.LoadExcelWorkbookIntoSqlite(book);
        _connection.Open();
    }


    [Test]
    public void TestGetTableList()
    {
        var tableNames = SqlUtils.GetTableNamesFromSqliteConnection(_connection);

        Assert.That(tableNames.Any(r => r == "albums"));
        Assert.That(tableNames.Any(r => r == "artists"));
        Assert.That(tableNames.Any(r => r == "customers"));
        Assert.That(tableNames.Any(r => r == "employees"));
        Assert.That(tableNames.Any(r => r == "genres"));
        Assert.That(tableNames.Any(r => r == "invoice_items"));
        Assert.That(tableNames.Any(r => r == "invoices"));
        Assert.That(tableNames.Any(r => r == "media_types"));
        Assert.That(tableNames.Any(r => r == "playlist_track"));
        Assert.That(tableNames.Any(r => r == "playlists"));
        Assert.That(tableNames.Any(r => r == "tracks"));
    }

    [Test]
    public void TestGetColumnListFromTable()
    {
        var columnNames = SqlUtils.GetColumnNamesForTableFromSqliteConnection(_connection, "customers");

        Assert.That(columnNames.Any(c => c == "CustomerId"));
        Assert.That(columnNames.Any(c => c == "FirstName"));
        Assert.That(columnNames.Any(c => c == "LastName"));
        Assert.That(columnNames.Any(c => c == "Company"));
        Assert.That(columnNames.Any(c => c == "Address"));
        Assert.That(columnNames.Any(c => c == "City"));
        Assert.That(columnNames.Any(c => c == "State"));
        Assert.That(columnNames.Any(c => c == "Country"));
        Assert.That(columnNames.Any(c => c == "PostalCode"));
        Assert.That(columnNames.Any(c => c == "Phone"));
        Assert.That(columnNames.Any(c => c == "Fax"));
        Assert.That(columnNames.Any(c => c == "Email"));
        Assert.That(columnNames.Any(c => c == "SupportRepId"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _connection.Close();
    }
}