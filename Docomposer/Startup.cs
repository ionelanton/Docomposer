using System.IO;
using Docomposer.Blazor.Services;
using Docomposer.Data.Databases.Util;
using Docomposer.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Docomposer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BlazorDataTransferService>();
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            // https://stackoverflow.com/a/61459904
            // https://docs.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-3.0&tabs=dotnet#configure-server-options
            services.AddSignalR(e => { e.MaximumReceiveMessageSize = 1024 * 1024 * 10; }); // 10MB
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime,
            BlazorDataTransferService dataTransferService, IDataProtectionProvider provider)
        {
            app.UseExceptionHandler("/api/error");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
            });

            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(env, provider));
        }

        private void OnApplicationStarted(IWebHostEnvironment env, IDataProtectionProvider provider)
        {
            ThisApp.FileHandler().CreatePath(ThisApp.DocReuseDocumentsPath());
            Directory.CreateDirectory(ThisApp.DocReuseCacheDirectory());

            if (env.IsDevelopment())
            {
                AppStoresInitializer.Initialize(true);
            }
        }
    }
}