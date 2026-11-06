using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    // Shown to the user taking the quiz. Never reveals correctness
    public class AnswerOptionDto
    {
        public int AnswerOptionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    // Shown to an admin managing questions. Includes correctness
    public class AnswerOptionAdminDto
    {
        public int AnswerOptionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class CreateAnswerOptionDto
    {
        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public bool IsCorrect { get; set; }
    }

    // AnswerOptionId is null for a newly added option during an update
    public class UpdateAnswerOptionDto
    {
        public int? AnswerOptionId { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public bool IsCorrect { get; set; }
    }
}