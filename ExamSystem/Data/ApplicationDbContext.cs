using ExamSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext ( DbContextOptions<ApplicationDbContext> options )
        : base(options)
    {
    }

    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Answer>()
       .HasOne(a => a.Question)
       .WithMany()
       .HasForeignKey(a => a.QuestionId)
       .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.SelectedOption)
            .WithMany()
            .HasForeignKey(a => a.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.StudentExam)
            .WithMany(se => se.Answers)
            .HasForeignKey(a => a.StudentExamId);
    }

    public DbSet<Exam> Exams { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<StudentExam> StudentExams { get; set; }
    public DbSet<Answer> Answers { get; set; }
}
