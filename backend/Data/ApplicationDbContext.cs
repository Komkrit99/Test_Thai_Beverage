using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ExamQuestion> ExamQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionNumber).IsRequired();
                entity.Property(e => e.QuestionText).IsRequired();
                entity.Property(e => e.Option1).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Option2).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Option3).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Option4).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CorrectAnswer).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.QuestionNumber);
            });
        }
    }
}