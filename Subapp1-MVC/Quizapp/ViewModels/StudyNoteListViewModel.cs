// Holds search filters and results for the study note overview.

using System.ComponentModel.DataAnnotations;

namespace Quizapp.ViewModels
{
    public class StudyNoteListViewModel
    {
        [Display(Name = "Search")]
        public string? SearchText { get; set; }

        [Display(Name = "Created from (UTC)")]
        [DataType(DataType.Date)]
        public DateTime? CreatedFrom { get; set; }

        public List<StudyNoteViewModel> Notes { get; set; } = new();
    }
}