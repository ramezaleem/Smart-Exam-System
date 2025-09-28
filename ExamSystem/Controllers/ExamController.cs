using ExamSystem.Data;
using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExamController ( ApplicationDbContext context )
        {
            _context = context;
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
    }
}
