namespace Quizapp.ViewModels
{   
    // Contains the information needed to display a quiz in the quiz overview.
    public class QuizListItemViewModel
    {
        public int QuizId { get; set; }

        public string PageTitle{ get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    // Contains the information needed to display the details of a selected quiz.
    public class QuizDetailsViewModel
    {
        public int QuizId { get; set; }

        public string PageTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int QuestionCount { get; set; }
    }

    // Contains the quiz and its questions while the user is taking the quiz.
    public class QuizPlayViewModel
    {
        public int QuizId { get; set; }

        public string PageTitle { get; set; } = string.Empty;

        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}