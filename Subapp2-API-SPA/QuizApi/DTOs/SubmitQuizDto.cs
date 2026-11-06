using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class SubmitQuizDto
    {
        [Required]
        public int QuizId { get; set; }

        [MinLength(1, ErrorMessage = "At least one answer must be submitted.")]
        public List<SubmittedAnswerDto> Answers { get; set; } = new();
    }

    public class SubmittedAnswerDto
    {
        [Required]
        public int QuestionId { get; set; }

        public List<int> SelectedAnswerOptionIds { get; set; } = new();
    }
}