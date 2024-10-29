using System.IO;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Databases.DataStore.Tables.Definitions;
using Docomposer.Data.Util;
using FluentMigrator;
using LinqToDB;
using LinqToDB.Data;

namespace Docomposer.Data.Databases.DataStore.Migrations
{
    [Migration(2)]
    public class M201911122111CreateTables : Migration
    {
        public override void Up()
        {
            using var db = new DocReuseDataConnection(ProviderName.SqlCe, DatabaseConnections.DataStoreConnectionString());

            db.CreateTable<TableUser>();
            db.CreateTable<TableRole>();
            db.CreateTable<TableUserRole>();
            db.CreateTable<TableProject>();
            db.CreateTable<TableDataSource>();
            db.CreateTable<TableSection>();
            db.CreateTable<TableDocument>();
            db.CreateTable<TableSectionDocument>();
            db.CreateTable<TableComposition>();
            db.CreateTable<TableDocumentComposition>();
            db.CreateTable<TableDataQuery>();
            db.CreateTable<TableWorkflow>();
            db.CreateTable<TableSetting>();
        }

        public override void Down()
        {
            Delete.Table(setting._);
            Delete.Table(workflow._);
            Delete.Table(data_query._);
            Delete.Table(document_x_composition._);
            Delete.Table(composition._);
            Delete.Table(section_x_document._);
            Delete.Table(document._);
            Delete.Table(section._);
            Delete.Table(data_source._);
            Delete.Table(project._);
            Delete.Table(user_role._);
            Delete.Table(role._);
            Delete.Table(user._);
        }
    }
}