using Microsoft.AspNetCore.Mvc;
using DapperApi.Models;
using DapperApi.Repositories;

namespace DapperApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repo;

    public StudentsController(IStudentRepository repo)
    {
        _repo = repo;
    }

    // GET /api/students
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repo.GetAll());
    }

    // GET /api/students/1
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var student = _repo.GetById(id);
        return student is null ? NotFound() : Ok(student);
    }

    // POST /api/students
    [HttpPost]
    public IActionResult Create([FromBody] Student student)
    {
        _repo.Create(student);
        return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
    }

    // PUT /api/students
    [HttpPut]
    public IActionResult Update([FromBody] Student student)
    {
        _repo.Update(student);
        return NoContent();
    }

    // DELETE /api/students/1
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _repo.Delete(id);
        return NoContent();
    }
    // GET /api/students/search?name=An
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string name)
    {
        var students = _repo.GetByName(name);
        return Ok(students);

    }
    // GET /api/students/courses
    [HttpGet("courses")]
    public IActionResult GetAllWithCourses()
    {
        var result = _repo.GetAllWithCourses();
        return Ok(result);
    }
}