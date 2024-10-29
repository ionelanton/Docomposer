using System.Threading.Tasks;
using Docomposer.Data.Util;
using Docomposer.Webdav;
using Microsoft.AspNetCore.Http;

namespace Docomposer.Core.Util.Middleware
{
    public class RequestWebDavPopulatorMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestWebDavPopulatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            new FilePopulator("").Populate();

            await _next(context);
        }
    }
}
