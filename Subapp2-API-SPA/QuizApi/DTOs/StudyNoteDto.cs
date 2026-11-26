// The DTOs folder contains the data sent between the API and the client.
// These classes describe a study note. UserId is left out on purpose,
// because the owner always comes from the token.

using System.ComponentModel.DataAnnotations;

namespace QuizApi.DTOs
{
    public class StudyNoteDto
    {
        public int StudyNoteId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    // Used both when creating and when updating a note, since the fields are the same.
    public class SaveStudyNoteDto
    {
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "The note content is required.")]
        [StringLength(5000, ErrorMessage = "The note cannot be longer than 5000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
