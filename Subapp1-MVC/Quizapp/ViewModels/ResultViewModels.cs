namespace Quizapp.ViewModels
{
    // Represents the selected answer options for one question.
    public class SubmitAnswerViewModel
    {
        public int QuestionId { get; set; }
        public List<int> SelectedAnswerOptionIds { get; set; } = new();
    }

    // Represents the whole submitted quiz.
    public class SubmitQuizViewModel
    {
        public int QuizId { get; set; }
        public List<SubmitAnswerViewModel> Answers { get; set; } = new();
    }

    // Contains the result and the server-calculated solution status.
    public class ResultViewModel
    {
        public int QuizAttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int AttemptCount { get; set; }
        public bool SolutionUnlocked { get; set; }
    }

    // Represents one question's result on the solution page.
    public class QuestionResultViewModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? SelectedAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    // Contains the questions displayed on the solution page.
    public class SolutionViewModel
    {
        public int QuizAttemptId { get; set; }
        public List<QuestionResultViewModel> Questions { get; set; } = new();
    }
}