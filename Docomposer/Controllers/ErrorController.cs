using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Docomposer.Controllers
{
    public class ErrorController : Controller
    {
        [Route("api/error")]
        public IActionResult Errors()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return Problem(title: "Error", detail: context.Error.Message);
        }
    }
}