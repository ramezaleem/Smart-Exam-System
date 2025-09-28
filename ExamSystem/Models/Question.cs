namespace ExamSystem.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int ExamId { get; set; }

    // جعل Exam nullable لتجنب مشاكل الربط مع POST
    public Exam? Exam { get; set; }

    // تهيئة للـ Options لمنع null
    public ICollection<Option> Options { get; set; } = new List<Option>();

    public int? Order { get; set; }
    public string? ImageUrl { get; set; }
}
