using Docomposer.Data.Databases.DataStore.Util;
using Docomposer.Data.Databases.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class MenusController
    {
        [HttpGet("Sections")]
        public string Sections()
        {
            return JsonConvert.SerializeObject(MenuBuilder.BuildFromTableSection());
        }
        
        [HttpGet("Documents")]
        public string Templates()
        {
            return JsonConvert.SerializeObject(MenuBuilder.BuildFromTableDocument());
        }
        
        [HttpGet("Compositions")]
        public string Compositions()
        {
            return JsonConvert.SerializeObject(MenuBuilder.BuildFromTableCompositions());
        }
        
        [HttpGet("Workflows")]
        public string Workflows()
        {
            return JsonConvert.SerializeObject(MenuBuilder.BuildFromTableWorkflows());
        }
        
        [HttpGet("DataSources")]
        public string DataSources()
        {
            return JsonConvert.SerializeObject(MenuBuilder.BuildFromTableDataSources());
        }
    }
}