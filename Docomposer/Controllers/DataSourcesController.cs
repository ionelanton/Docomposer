using System;
using Docomposer.Core.Api;
using Docomposer.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docomposer.Controllers
{
    [Route("api/[controller]")]
    public class DataSourcesController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id:int}")]
        public IActionResult Get(int id, [FromQuery]string p)
        {
            try
            {
                var dataSource = DataSources.GetDataSourceById(id);
                return Ok(dataSource);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPost]
        public IActionResult Post([FromBody]DataSource dataSource)
        {
            try
            {
                return Ok(DataSources.AddDataSource(dataSource));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // PUT api/<controller>/5
        [HttpPut("WithFile/{id:int}")]
        public IActionResult UpdateDataSourceWithFile(int id, [FromBody]DataSourceWithFile dataSourceWithFile)
        {
            try
            {
                if (id == dataSourceWithFile.DataSource.Id)
                {
                    DataSources.UpdateDataSourceWithFile(dataSourceWithFile);
                    return Ok();
                }

                return BadRequest(dataSourceWithFile.DataSource.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPut("{id:int}")]
        public IActionResult UpdateDataSource(int id, [FromBody]DataSource dataSource)
        {
            try
            {
                if (id == dataSource.Id)
                {
                    DataSources.UpdateDataSourceById(dataSource);
                    return Ok();
                }

                return BadRequest(dataSource.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                DataSources.DeleteDataSource(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}