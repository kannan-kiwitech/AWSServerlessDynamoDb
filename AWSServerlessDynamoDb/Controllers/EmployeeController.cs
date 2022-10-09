using AWSServerlessDynamoDb.Models;
using AWSServerlessDynamoDb.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AWSServerlessDynamoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeDb _employeeDb;

        public EmployeeController(IEmployeeDb employeeDb)
        {
            _employeeDb = employeeDb;
        }

        [HttpGet("{empCode}")]
        public async Task<IActionResult> Get(string empCode)
        {
            var result = await _employeeDb.GetEmployeeAsync(empCode);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("reportees/{empCode}")]
        public async Task<IActionResult> GetReportees(string empCode)
        {
            var result = await _employeeDb.GetAllReporteesAsync(empCode);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpsertEmployee([FromBody] EmployeeModel model)
        {
            await _employeeDb.SaveAsync(model);
            return Ok();
        }
    }
}