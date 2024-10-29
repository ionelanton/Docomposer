using System;
using Docomposer.Core.Util;
using Docomposer.Data.Util;
using Docomposer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Docomposer
{
    public class JsConfig
    {
        public const string ApiTokenSessionValue = "ApiTokenSessionValue";
        public string apiToken { get; set; }
        public string filesPath { get; set; }
        
        public static string AsJson(PageModel model)
        {
            var token = model.HttpContext.Session.GetString(ApiTokenSessionValue);

            if (token == null && ThisApp.AppEnvironment() == "Development")
            {
                token = "development-api-token";
            }
            
            return JsonConvert.SerializeObject(new JsConfig
            {
                apiToken = token,
                filesPath = ThisApp.DocReuseDocumentsPath()
            });
        }
    }
}