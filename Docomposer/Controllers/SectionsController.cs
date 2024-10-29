// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System;
using System.Collections.Generic;
using Docomposer.Core.Api;
using Microsoft.AspNetCore.Mvc;
using Docomposer.Core.Domain;
using Microsoft.AspNetCore.Http;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class SectionsController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromQuery]bool refresh = false)
        {
            try
            {
                var base64 = Sections.GetSectionAsPdf(id, refresh);
                return Ok(base64);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Section section)
        {
            try
            {
                return Ok(Sections.AddSection(section));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Section section)
        {
            try
            {
                if (id == section.Id)
                {
                    Sections.UpdateSection(section);
                    return Ok();
                }

                return BadRequest(section.Name);
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
                Sections.DeleteSection(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
