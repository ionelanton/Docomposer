using System.Diagnostics;
using System.IO;
using Docomposer.Blazor.Services;
using Docomposer.Utils;
using Microsoft.AspNetCore.Mvc;
using RunProcessAsTask;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class Util : Controller
    {
        private readonly BlazorDataTransferService _service;

        public Util(BlazorDataTransferService service)
        {
            _service = service;
        }

        [HttpGet("download")]
        public IActionResult Get([FromQuery] string path)
        {
            var stream = System.IO.File.OpenRead(path);
            return File(stream, "application/octet-stream", Path.GetFileName(path));
        }
        
        

        [HttpGet("explore")]
        public void Open([FromQuery] string path)
        {
            var process = Process.Start("explorer.exe", $"/select,\"{path}\"");
        }

        [HttpPut]
        public IActionResult Put([FromBody] BlazorTransferData blazorTransferData)
        {
            _service.TransferData = blazorTransferData;
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] DocumentRequest request)
        {
            ProcessEx.RunAsync(new ProcessStartInfo
            {
                FileName = ThisApp.MicrosoftOfficeWord(),
                Arguments =
                    $"/t \"{ThisApp.DocReuseDocumentsPath()}/{request.ProjectId}/{request.Type}/{request.Name}.docx\""
            });

            return Ok();
        }

        [HttpGet("check/{projectId}/{type}/{name}")]
        public IActionResult Get(string projectId, string type, string name)
        {
            var path = $"{ThisApp.DocReuseDocumentsPath()}/{projectId}/{type}";

            return Ok(new FileSystemHandler().IsWordDocumentOpen(path, name) ? "true" : "false");
        }
    }

    public class DocumentRequest
    {
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}