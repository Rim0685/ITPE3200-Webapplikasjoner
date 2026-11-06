namespace QuizApi.DTOs
{
    public class QuizSolutionDto
    {
        public int QuizAttemptId { get; set; }
        public List<QuestionSolutionDto> Questions { get; set; } = new();
    }

    public class QuestionSolutionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<int> SelectedAnswerOptionIds { get; set; } = new();
        public List<int> CorrectAnswerOptionIds { get; set; } = new();
        public bool WasFullyCorrect { get; set; }
    }
}