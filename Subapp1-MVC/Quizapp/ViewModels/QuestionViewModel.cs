namespace Quizapp.ViewModels
{
    // Used to send one question and its answer options to the view.
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        
        public int QuestionIndex { get; set; }

        public bool IsMultipleChoice { get; set; }

        public bool IsImageQuestion { get; set; }
        
        public List<AnswerOptionViewModel> Options { get; set; } = new();

        public List<int> SelectedOptionIds { get; set; } = new();
    }

    // Represents one answer option shown in the quiz.
    public class AnswerOptionViewModel
    {
        public int Id { get; set; }

        public string? Text { get; set; }

        public string? ImageUrl { get; set; }
    }
}