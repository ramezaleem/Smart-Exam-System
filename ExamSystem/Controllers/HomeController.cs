using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ExamSystem.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController ( ILogger<HomeController> logger )
        {
            _logger = logger;
        }

        public IActionResult Index ()
        {
            return View();
        }

        public IActionResult Privacy ()
        {
            return View();
        }

        [AllowAnonymous] // ���� ������ ����� ��� ���������� �� ����� ����� ��� �������
        public IActionResult Welcome ()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name ?? "���";
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                ViewBag.UserName = userName;
                ViewBag.UserRoles = roles;
            }
            else
            {
                ViewBag.UserName = "���";
                ViewBag.UserRoles = new List<string>();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
