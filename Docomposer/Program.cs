using Docomposer.Data.Databases.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Docomposer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "setup")
            {
                AppStoresInitializer.Initialize(true);
            }
            else if (args.Length == 2 && args[0] == "setup" && args[1] == "force")
            {
                AppStoresInitializer.Initialize(true);
            }
            else
            {
                var host = CreateHostBuilder(args).Build();
                host.Run();
            }
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}