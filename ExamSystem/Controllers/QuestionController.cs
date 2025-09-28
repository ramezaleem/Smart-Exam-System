using ExamSystem.Data;
using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public QuestionController ( ApplicationDbContext context )
        {
            _context = context;
        }

        // عرض قائمة أسئلة الامتحان
        public async Task<IActionResult> Index ( int examId )
        {
            var exam = await _context.Exams.Include(e => e.Questions).ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == examId);
            if (exam == null)
                return NotFound();
            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamId = exam.Id;
            return View(exam.Questions.ToList());
        }

        // عرض صفحة إضافة سؤال جديد
        public IActionResult Create ( int examId )
        {
            ViewBag.ExamId = examId;
            return View();
        }

        // حفظ سؤال جديد مع خياراته
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( int examId, Question question, List<Option> options )
        {
            if (ModelState.IsValid && options.Count >= 2 && options.Any(o => o.IsCorrect))
            {
                question.ExamId = examId;
                question.Options = options;
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { examId });
            }
            ViewBag.ExamId = examId;
            ModelState.AddModelError("", "يجب إدخال على الأقل اختيارين وتحديد إجابة صحيحة واحدة.");
            return View(question);
        }

        // تفاصيل سؤال
        public async Task<IActionResult> Details ( int? id )
        {
            if (id == null)
                return NotFound();
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound();
            return View(question);
        }

        // تعديل سؤال
        public async Task<IActionResult> Edit ( int? id )
        {
            if (id == null)
                return NotFound();
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, Question question, List<Option> options )
        {
            if (id != question.Id)
                return NotFound();
            if (ModelState.IsValid && options.Count >= 2 && options.Any(o => o.IsCorrect))
            {
                var oldQuestion = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
                if (oldQuestion == null)
                    return NotFound();
                oldQuestion.Text = question.Text;
                // حذف الخيارات القديمة
                _context.Options.RemoveRange(oldQuestion.Options);
                // إضافة الخيارات الجديدة
                oldQuestion.Options = options;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { examId = oldQuestion.ExamId });
            }
            ModelState.AddModelError("", "يجب إدخال على الأقل اختيارين وتحديد إجابة صحيحة واحدة.");
            return View(question);
        }

        // حذف سؤال
        public async Task<IActionResult> Delete ( int? id )
        {
            if (id == null)
                return NotFound();
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound();
            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question != null)
            {
                _context.Options.RemoveRange(question.Options);
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { examId = question?.ExamId });
        }
    }
}
