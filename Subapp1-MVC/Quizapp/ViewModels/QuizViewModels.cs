namespace Quizapp.ViewModels
{
    public class QuizListItemViewModel
    {
        public int QuizId { get; set;}

        public string PageTitle{ get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class QuizDetailsViewModel
    {
        public int QuizId { get; set; }

        public string PageTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int QuestionCount { get; set; }
    }
}