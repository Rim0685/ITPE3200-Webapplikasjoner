// The DAL folder contains database configuration and data access code.
// This interface defines database operations for a user's study notes.

using QuizApi.Models;

namespace QuizApi.DAL
{
    public interface IStudyNoteRepository
    {
        Task<IEnumerable<StudyNote>> GetAllAsync(string userId);

        // Returns null if the note does not exist or belongs to another user.
        Task<StudyNote?> GetByIdAsync(int noteId, string userId);

        Task<bool> CreateAsync(StudyNote note);

        // Returns false if the note was not found or the save failed.
        Task<bool> UpdateAsync(StudyNote note, string userId);

        Task<bool> DeleteAsync(int noteId, string userId);
    }
}
