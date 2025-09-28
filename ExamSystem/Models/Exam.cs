namespace ExamSystem.Models;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Description { get; set; }
    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
}
