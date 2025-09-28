using ExamSystem.Data;
using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StudentExamController ( ApplicationDbContext context )
        {
            _context = context;
        }

        // عرض كل الامتحانات المتاحة
        public async Task<IActionResult> AvailableExams ()
        {
            var exams = await _context.Exams.Include(e => e.Questions).ToListAsync();
            return View(exams);
        }

        // بدء الامتحان
        public async Task<IActionResult> Start ( int examId )
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var alreadyTaken = await _context.StudentExams
                .AnyAsync(se => se.ExamId == examId && se.StudentId == userId);

            if (alreadyTaken)
                return RedirectToAction("Result", new { examId });

            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit ( int examId, Dictionary<int, int> selectedOptionIds )
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var exam = await _context.Exams
                .Include(e => e.Questions).ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return NotFound();

            var studentExam = new StudentExam
            {
                StudentId = userId,
                ExamId = examId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            _context.StudentExams.Add(studentExam);
            await _context.SaveChangesAsync();

            int correct = 0;

            foreach (var pair in selectedOptionIds)
            {
                int questionId = pair.Key;
                int selectedOptId = pair.Value;

                var option = await _context.Options.FindAsync(selectedOptId);
                if (option != null && option.IsCorrect)
                    correct++;

                var answer = new Answer
                {
                    StudentExamId = studentExam.Id,
                    QuestionId = questionId,
                    SelectedOptionId = selectedOptId,
                    AnsweredAt = DateTime.Now
                };
                _context.Answers.Add(answer);
            }

            studentExam.Score = correct;
            studentExam.Percentage = (double)correct / exam.Questions.Count * 100;
            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { examId });
        }

        // عرض النتيجة
        public async Task<IActionResult> Result ( int examId )
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var studentExam = await _context.StudentExams
                .Include(se => se.Exam)
                .Include(se => se.Answers).ThenInclude(a => a.Question)
                .Include(se => se.Answers).ThenInclude(a => a.SelectedOption)
                .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == userId);

            if (studentExam == null)
                return NotFound();

            var allQuestions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ExamId == examId).ToListAsync();

            ViewBag.AllQuestions = allQuestions;
            return View(studentExam);
        }
    }
}
