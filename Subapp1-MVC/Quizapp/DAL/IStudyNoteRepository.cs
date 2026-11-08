// The DAL folder contains database configuration and data access code.
// This interface defines database operations for a user's study notes.

using Quizapp.Models;

namespace Quizapp.DAL
{
    public interface IStudyNoteRepository
    {
        // Retrieves all notes belonging to the specified user.
        Task<IEnumerable<StudyNote>> GetAll(string userId);

        // Retrieves a note only if it belongs to the specified user.
        Task<StudyNote?> GetById(int id, string userId);

        // Saves a new study note.
        Task Create(StudyNote studyNote);

        // Updates an existing note belonging to the specified user.
        Task<bool> Update(StudyNote studyNote, string userId);

        // Deletes a note only if it belongs to the specified user.
        Task<bool> Delete(int id, string userId);
    }
}