using StudentScoreApi.Models;

namespace StudentScoreApi.Services;

public class StudentService
{
    private readonly List<Student> _students = new();

    public StudentService()
    {
        // Seed 10 students with random scores
        var subjects = new[] { "Math", "Science", "English", "History", "Art" };
        var random = new Random();

        for (int i = 1; i <= 10; i++)
        {
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = $"Student {i}"
            };

            foreach (var subject in subjects)
            {
                student.Scores.Add(new Score
                {
                    Subject = subject,
                    Value = random.Next(50, 101) // Score between 50 and 100
                });
            }

            _students.Add(student);
        }
    }

    public List<Student> GetAll() => _students;
    
    public Student? GetById(Guid id) => _students.FirstOrDefault(s => s.Id == id);
}
