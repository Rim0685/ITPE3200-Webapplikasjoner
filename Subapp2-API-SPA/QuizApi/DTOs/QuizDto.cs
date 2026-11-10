using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class QuizDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public decimal? LastScore { get; set; }
        public decimal? LastMaxScore { get; set; }
        public int AttemptCount { get; set; }
        public bool IsSolutionUnlocked { get; set; }
    }

    // Served when a user starts/takes the quiz. Never includes IsCorrect
    public class QuizTakeDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuestionTakeDto> Questions { get; set; } = new();
    }

    public class QuestionTakeDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool AllowMultipleAnswers { get; set; }
        public string? ImageUrl { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}