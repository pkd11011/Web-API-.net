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

            // Kiểm tra nếu có ảnh
            if (!string.IsNullOrWhiteSpace(student.Photo))
            {
                if (!student.Photo.StartsWith("data:image"))
                {
                    return BadRequest(new { message = "Invalid image format. Must be Base64 encoded." });
                }
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Created("", student);
        }
        [HttpPut("UpdatePhoto/{id}")]
        public async Task<IActionResult> UpdatePhoto(int id, [FromBody] string base64ImageString)
        {
            if (string.IsNullOrWhiteSpace(base64ImageString))
            {
                return BadRequest(new { message = "Ảnh không được để trống!" });
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên!" });
            }

            if (!base64ImageString.StartsWith("data:image"))
            {
                return BadRequest(new { message = "Invalid image format. Must be Base64 encoded." });
            }

            student.Photo = base64ImageString;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ảnh đã cập nhật thành công!", student });
        }
    }
}
