using ExamSystem.Data;
using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController ( ApplicationDbContext context, UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard ()
        {
            var examsCount = await _context.Exams.CountAsync();

            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var students = await _userManager.GetUsersInRoleAsync("Student");

            var questionsCount = await _context.Questions.CountAsync();

            ViewBag.ExamsCount = examsCount;
            ViewBag.TeachersCount = teachers.Count;
            ViewBag.StudentsCount = students.Count;
            ViewBag.QuestionsCount = questionsCount;

            return View();
        }

        public async Task<IActionResult> Teachers ()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            return View(teachers);
        }

        public async Task<IActionResult> Students ()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            return View(students);
        }

        // حذف مدرس
        [HttpPost]
        public async Task<IActionResult> DeleteTeacher ( string id )
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var teacher = await _userManager.FindByIdAsync(id);
            if (teacher == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(teacher);
            if (result.Succeeded)
                return RedirectToAction(nameof(Teachers));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(nameof(Teachers), await _userManager.GetUsersInRoleAsync("Teacher"));
        }

        // حذف طالب
        [HttpPost]
        public async Task<IActionResult> DeleteStudent ( string id )
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(student);
            if (result.Succeeded)
                return RedirectToAction(nameof(Students));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(nameof(Students), await _userManager.GetUsersInRoleAsync("Student"));
        }
    }
}
