using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public bool AllowMultipleAnswers { get; set; }
        public string? ImageUrl { get; set; }
        public List<AnswerOptionAdminDto> AnswerOptions { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Text { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Explanation { get; set; } = string.Empty;

        public bool AllowMultipleAnswers { get; set; }
        public string? ImageUrl { get; set; }

        [MinLength(2, ErrorMessage = "A question needs at least 2 answer options.")]
        public List<CreateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Text { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Explanation { get; set; } = string.Empty;

        public bool AllowMultipleAnswers { get; set; }
        public string? ImageUrl { get; set; }

        [MinLength(2, ErrorMessage = "A question needs at least 2 answer options.")]
        public List<UpdateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}