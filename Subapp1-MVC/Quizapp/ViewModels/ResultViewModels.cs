namespace Quizapp.ViewModels
{
    // Representerer ett svar eleven har valgt for ett spørsmål.
    public class SubmitAnswerViewModel
    {
        public int QuestionId { get; set; }
        public List<int> SelectedAnswerOptionIds { get; set; } = new();
    }

    // Representerer hele den innsendte quizen.
    public class SubmitQuizViewModel
    {
        public int QuizId { get; set; }
        public List<SubmitAnswerViewModel> Answers { get; set; } = new();
    }

    // Det som vises på resultatsiden.
    public class ResultViewModel
    {
        public int QuizAttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }

    // Én rad i oversikten på fasitsiden.
    public class QuestionResultViewModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? SelectedAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    // Det som vises på fasitsiden.
    public class SolutionViewModel
    {
        public int QuizAttemptId { get; set; }
        public List<QuestionResultViewModel> Questions { get; set; } = new();
    }
}