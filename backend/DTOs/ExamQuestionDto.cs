using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class ExamQuestionDto
    {
        public int Id { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public string Option4 { get; set; } = string.Empty;
        public int CorrectAnswer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateExamQuestionDto
    {
        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 1 is required")]
        [MaxLength(500, ErrorMessage = "Option 1 cannot exceed 500 characters")]
        public string Option1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 2 is required")]
        [MaxLength(500, ErrorMessage = "Option 2 cannot exceed 500 characters")]
        public string Option2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 3 is required")]
        [MaxLength(500, ErrorMessage = "Option 3 cannot exceed 500 characters")]
        public string Option3 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 4 is required")]
        [MaxLength(500, ErrorMessage = "Option 4 cannot exceed 500 characters")]
        public string Option4 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct answer is required")]
        [Range(1, 4, ErrorMessage = "Correct answer must be between 1 and 4")]
        public int CorrectAnswer { get; set; }
    }

    public class UpdateExamQuestionDto
    {
        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 1 is required")]
        [MaxLength(500, ErrorMessage = "Option 1 cannot exceed 500 characters")]
        public string Option1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 2 is required")]
        [MaxLength(500, ErrorMessage = "Option 2 cannot exceed 500 characters")]
        public string Option2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 3 is required")]
        [MaxLength(500, ErrorMessage = "Option 3 cannot exceed 500 characters")]
        public string Option3 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option 4 is required")]
        [MaxLength(500, ErrorMessage = "Option 4 cannot exceed 500 characters")]
        public string Option4 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct answer is required")]
        [Range(1, 4, ErrorMessage = "Correct answer must be between 1 and 4")]
        public int CorrectAnswer { get; set; }
    }
}