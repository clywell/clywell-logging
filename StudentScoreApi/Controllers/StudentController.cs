using Microsoft.AspNetCore.Mvc;
using StudentScoreApi.Models;
using StudentScoreApi.Services;
using Clywell.Core.Logging.Extensions;
using System.Text.Json;

namespace StudentScoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;
    private readonly ILogger<StudentController> _logger;

    public StudentController(StudentService studentService, ILogger<StudentController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<List<Student>> GetAll()
    {
        using (_logger.BeginTimedScope("GetAllStudents", new Dictionary<string, object>()))
        {
            _logger.Info("Retrieving all students");
            
            var students = _logger.LogExecutionTime("FetchStudentsFromService", () => 
            {
                return _studentService.GetAll();
            });

            _logger.Info("Retrieved {StudentCount} students", students.Count);
            
            return Ok(students);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Student> GetById(Guid id)
    {
        using (_logger.BeginTimedScope("GetStudentById", new Dictionary<string, object> { ["StudentId"] = id }))
        {
            var student = _studentService.GetById(id);
            
            if (student == null)
            {
                _logger.Warning("Student with ID {StudentId} not found", id);
                return NotFound();
            }

            _logger.Info("Found student {StudentName}", student.Name);
            
            // Using Clywell's Debug extension which includes IsEnabled check
            // However, to strictly optimize the serialization, we check manually too if expensive
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.Debug("Full student data: {Json}", JsonSerializer.Serialize(student));
            }

            return Ok(student);
        }
    }
}
