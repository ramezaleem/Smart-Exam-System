using ExamSystem.Data;
using ExamSystem.Models;
using ExamSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ExamController ( ApplicationDbContext context, UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Exam
        public async Task<IActionResult> Index ()
        {
            var exams = await _context.Exams.Include(e => e.Questions).ToListAsync();
            return View(exams);
        }

        // GET: Exam/Details/5
        public async Task<IActionResult> Details ( int? id )
        {
            if (id == null)
                return NotFound();
            var exam = await _context.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null)
                return NotFound();
            return View(exam);
        }

        // GET: Exam/Create
        public IActionResult Create ()
        {
            return View();
        }

        // POST: Exam/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( [Bind("Title,StartTime,EndTime,Description")] Exam exam )
        {
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                // توجه لإضافة اسئلة الامتحان الجديد
                return RedirectToAction("Index", "Question", new { examId = exam.Id });
            }
            return View(exam);
        }

        // GET: Exam/Edit/5
        public async Task<IActionResult> Edit ( int? id )
        {
            if (id == null)
                return NotFound();
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return NotFound();
            return View(exam);
        }

        // POST: Exam/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, [Bind("Id,Title,StartTime,EndTime,Description")] Exam exam )
        {
            if (id != exam.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exams.Any(e => e.Id == exam.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exam);
        }

        // GET: Exam/Delete/5
        public async Task<IActionResult> Delete ( int? id )
        {
            if (id == null)
                return NotFound();
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null)
                return NotFound();
            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> StudentScores ( int examId )
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return NotFound();

            var teacherId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var students = await _userManager.GetUsersInRoleAsync("Student");

            var studentExamScores = await _context.StudentExams
                .Where(se => se.ExamId == examId)
                .ToListAsync();

            int totalQuestions = exam.Questions?.Count ?? 0;

            var studentScoresList = students.Select(s =>
            {
                var scoreRecord = studentExamScores.FirstOrDefault(se => se.StudentId == s.Id);

                double? percentage = null;
                if (scoreRecord != null && totalQuestions > 0)
                {
                    percentage = ((double)scoreRecord.Score / totalQuestions) * 100;
                }

                return new StudentScoreItem
                {
                    StudentName = s.UserName,
                    Score = scoreRecord?.Score,
                    Status = (scoreRecord != null && scoreRecord.Score > 0) ? "امتحن" : "لم يمتحن بعد",
                    Percentage = percentage
                };
            }).ToList();

            var totalStudents = students.Count;
            var studentsTaken = studentScoresList.Count(ss => ss.Score > 0);
            var percentageTaken = totalStudents == 0 ? 0 : (studentsTaken * 100) / totalStudents;

            var viewModel = new ExamScoresViewModel
            {
                ExamTitle = exam.Title,
                TotalStudents = totalStudents,
                StudentsTaken = studentsTaken,
                PercentageTaken = percentageTaken,
                StudentScores = studentScoresList
            };

            return View(viewModel);
        }





    }
}
