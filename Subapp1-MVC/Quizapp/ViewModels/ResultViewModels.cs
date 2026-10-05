namespace Quizapp.ViewModels
{
    // Represents the answer the student selected for one question.
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

    // What is shown on the result page.
    public class ResultViewModel
    {
        public int QuizAttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }

    // One row in the overview on the solution page.
    public class QuestionResultViewModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? SelectedAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    // What is shown on the solution page.
    public class SolutionViewModel
    {
        public int QuizAttemptId { get; set; }
        public List<QuestionResultViewModel> Questions { get; set; } = new();
    }
}