using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("exam_questions")]
    public class ExamQuestion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("question_number")]
        public int QuestionNumber { get; set; }

        [Required]
        [Column("question_text")]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [Column("option1")]
        [MaxLength(500)]
        public string Option1 { get; set; } = string.Empty;

        [Required]
        [Column("option2")]
        [MaxLength(500)]
        public string Option2 { get; set; } = string.Empty;

        [Required]
        [Column("option3")]
        [MaxLength(500)]
        public string Option3 { get; set; } = string.Empty;

        [Required]
        [Column("option4")]
        [MaxLength(500)]
        public string Option4 { get; set; } = string.Empty;

        [Required]
        [Column("correct_answer")]
        [Range(1, 4)]
        public int CorrectAnswer { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}