using System;
using System.Data;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;

namespace Docomposer.Data.Test;

[TestFixture]
public class TestCsvHelper
{
    [Ignore("Manual test")]
    [Test]
    public void TestRead()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant().Trim().Replace(" ", "_"),
        };
        
        using var reader = new StreamReader(@"C:\Temp\sakila\actor.csv");
        using var csv = new CsvReader(reader, config);
        
        using var dr = new CsvDataReader(csv);
        var dt = new DataTable();
        dt.Load(dr);

        foreach (var c in dt.Columns)
        {
            Console.WriteLine(c);
        }
    }
}