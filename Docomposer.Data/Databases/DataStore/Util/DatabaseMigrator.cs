using System;
using Docomposer.Data.Util;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public class DatabaseMigrator
    {
        private static string _connectionString;
        
        public DatabaseMigrator()
        {
            _connectionString = DatabaseConnections.DataStoreConnectionString();
        }

        public DatabaseMigrator(IConfiguration config)
        {
            _connectionString = config["key:value"];
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public void Execute()
        {
            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        public void MigrateUp(long version)
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                RunnerMigrateUp(scope.ServiceProvider, version);
            }
        }

        public void MigrateDown(long version)
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                RunnerMigrateDown(scope.ServiceProvider, version);
            }
        }

        public void CleanUp()
        {
            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                CleanupDatabase(scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices()
        {
            //var composition = typeof(DatabaseSettings).GetTypeInfo().Assembly;

            var assembly = typeof(DatabaseSettings).Assembly; // new AssemblyHelper().GetType().Assembly;


            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add Postgres support to FluentMigrator
                    //.AddPostgres()
                    // Add Sqlite support to FluentMigrator
                    //.AddSQLite()
                    .AddSqlServerCe()
                    // Set the connection string
                    .WithGlobalConnectionString(_connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(assembly).For.Migrations())

                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        /// <summary>
        /// Cleanup the database
        /// </summary>
        private static void CleanupDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(1);
        }

        private static void RunnerMigrateUp(IServiceProvider serviceProvider, long version)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp(version);
        }

        private static void RunnerMigrateDown(IServiceProvider serviceProvider, long version)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(version);
        }
    }
}