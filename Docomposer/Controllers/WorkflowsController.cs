using System;
using Docomposer.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class WorkflowsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromQuery]bool refresh = false)
        {
            try
            {
                var task = Core.Api.Workflows.GetWorkflowById(id);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Workflow workflow)
        {
            try
            {
                return Ok(Core.Api.Workflows.AddWorkflow(workflow));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Workflow workflow)
        {
            try
            {
                if (id == workflow.Id)
                {
                    var storedTask = Core.Api.Workflows.GetWorkflowById(id);
                    storedTask.Name = workflow.Name;
                    Core.Api.Workflows.UpdateWorkflow(storedTask);
                    return Ok();
                }
        
                return BadRequest(workflow.Name);
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
                Core.Api.Workflows.DeleteWorkflowById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}