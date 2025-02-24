using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _context;

        public StudentsController(StudentContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            if (students.Count == 0)
            {
                return NotFound(new { message = "No students found" });
            }
            return Ok(students);
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] Student student)
        {
            if (student == null || string.IsNullOrWhiteSpace(student.Name) || string.IsNullOrWhiteSpace(student.Class))
            {
                return BadRequest(new { message = "Invalid student data. Name and Class are required." });
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Created("", student);
        }
    }
}
