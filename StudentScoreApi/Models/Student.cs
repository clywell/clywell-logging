namespace StudentScoreApi.Models;

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Score> Scores { get; set; } = new();
}
