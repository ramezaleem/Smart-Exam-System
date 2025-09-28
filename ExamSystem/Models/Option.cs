namespace ExamSystem.Models;

public class Option
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }

    // جعل Question nullable لتجنب مشاكل الربط مع POST
    public Question? Question { get; set; }
}
