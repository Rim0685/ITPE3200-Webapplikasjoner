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
}