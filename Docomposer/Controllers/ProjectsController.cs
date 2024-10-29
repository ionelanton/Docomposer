using System;
using System.Reflection.Metadata;
using Docomposer.Core.Api;
using Microsoft.AspNetCore.Mvc;
using Docomposer.Core.Domain;
using Microsoft.AspNetCore.Http;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]Project project)
        {
            try
            {
                return Ok(Projects.AddProject(project.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Project project)
        {
            try
            {
                if (id == project.Id && ModelState.IsValid)
                {
                    Projects.UpdateProject(project);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return BadRequest(project.Name);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Projects.DeleteProject(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
            return Ok();
        }
    }
}