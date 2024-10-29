using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Docomposer
{
    public static class WebAppUtils
    {
        public static string AsActiveCssAttribute(PageModel model, string partial)
        {
            if (model.HttpContext.Request.Path.ToString().ToLower().Contains(partial))
            {
                return "active";
            }

            return "";
        }
    }
}