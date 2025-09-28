namespace ExamSystem.Models;

public class StudentExam
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }
    public int ExamId { get; set; }
    public Exam Exam { get; set; }
    public double Score { get; set; }
    public double Percentage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ICollection<Answer> Answers { get; set; }
    public string? Feedback { get; set; }
}
