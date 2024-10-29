using System;
using Microsoft.AspNetCore.Mvc;
using Docomposer.Core.Domain;
using Docomposer.Core.Api;
using Microsoft.AspNetCore.Http;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class CompositionsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromQuery]bool refresh = false)
        {
            try
            {
                var base64 = Compositions.GetCompositionAsPdf(id, refresh);
                return Ok(base64);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Composition composition)
        {
            try
            {
                return Ok(Compositions.AddComposition(composition));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Composition composition)
        {
            try
            {
                if (id == composition.Id)
                {
                    Compositions.UpdateComposition(composition);
                    return Ok();
                }

                return BadRequest(composition.Name);
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
                Compositions.DeleteComposition(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}