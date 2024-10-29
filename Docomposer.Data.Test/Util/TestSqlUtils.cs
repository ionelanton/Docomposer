using System;
using System.Collections.Generic;
using Docomposer.Data.Util;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Docomposer.Data.Databases.DataStore.Tables;
using NUnit.Framework;

namespace Docomposer.Data.Test.Util
{
    [TestFixture]
    public class TestSqlUtils
    {
        [Test]
        public void TestGetSqlParametersFromStatement()
        {
            var statement = @"SELECT * FROM 'table' WHERE gender = @param1 or Gender = @param2";

            var result = SqlUtils.GetSqlParametersFromStatement(statement);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Exists(p => p.Name == "param1"));
            Assert.That(result.Exists(p => p.Name == "param2"));
        }

        [Test]
        public void TestCreateDbCommandWithParameters()
        {
            DbCommand cmd = null;
            const string statement = @"SELECT * FROM 'table' WHERE gender = @param1 or surname = @param2";
            var sqlParams = new List<SqlParam>
            {
                new SqlParam { Name = "param1", Value = "value1" },
                new SqlParam { Name = "param2", Value = "value2" }
            };

            using var connection = new SqliteConnection("Data Source=:memory:;");
            connection.Open();
            cmd = connection.CreateDbCommandWithParameters(DataSourceEnum.Sqlite, statement, sqlParams);
            connection.Close();

            Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
            
            Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("@param1"));
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo("value1"));
            
            Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("@param2"));
            Assert.That(cmd.Parameters[1].Value, Is.EqualTo("value2"));
        }

        [Test]
        public void TestSqliteMsConnectionBuilder()
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = true,
                DataSource = "file_path"
            }.ToString();
            Console.Out.WriteLine("connectionString = {0}", connectionString);
        }

        [Test]
        public void TestSanitizeSquareBrackets()
        {
            Assert.That(SqlUtils.SanitizeSquareBracketsParamValues("[tableName].[fieldName]"), Is.EqualTo("[tableName].[fieldName]"));
            Assert.That(SqlUtils.SanitizeSquareBracketsParamValues("[table name].[field name]"), Is.EqualTo("[table name].[field name]"));
            Assert.That(SqlUtils.SanitizeSquareBracketsParamValues("  [ table name ] .  [ field name ]"), Is.EqualTo("[table name].[field name]"));
            Assert.That(SqlUtils.SanitizeSquareBracketsParamValues(" [  a table name ] . [ a field name]   "), Is.EqualTo("[a table name].[a field name]"));
        }
    }
}