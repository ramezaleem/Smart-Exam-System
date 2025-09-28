using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
}
