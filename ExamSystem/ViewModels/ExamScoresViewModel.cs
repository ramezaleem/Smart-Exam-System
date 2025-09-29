namespace ExamSystem.ViewModels;

public class ExamScoresViewModel
{
    public string ExamTitle { get; set; }
    public int TotalStudents { get; set; }
    public int StudentsTaken { get; set; }
    public int PercentageTaken { get; set; }
    public List<StudentScoreItem> StudentScores { get; set; }
}