namespace QuizApi.DTOs
{
    public class QuizResultDto
    {
        public int QuizAttemptId { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public int AttemptCount { get; set; }
        public bool IsSolutionUnlocked { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public bool WasFullyCorrect { get; set; }
        public decimal AwardedPoints { get; set; }
    }
}