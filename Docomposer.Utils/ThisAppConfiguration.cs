using System;
using Microsoft.Extensions.Configuration;

namespace Docomposer.Utils
{
    public static class ThisAppConfiguration
    {
        public static IConfiguration Configuration { get; }

        static ThisAppConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
    }
}