using System;
using Docomposer.Core.Domain;
using Docomposer.Core.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromQuery]bool refresh)
        {
            try
            {
                var base64 = Documents.GetDocumentAsPdf(id, refresh);
                return Ok(base64);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Document document)
        {
            try
            {
                return Ok(Documents.AddDocument(document));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Document document)
        {
            try
            {
                if (id == document.Id)
                {
                    Documents.UpdateDocument(document);
                    return Ok();
                }

                return BadRequest(document.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Documents.DeleteDocument(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}