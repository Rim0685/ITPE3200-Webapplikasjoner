// The DAL folder contains database configuration and data access code.
// This class implements database operations for a user's study notes.

using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    public class StudyNoteRepository : IStudyNoteRepository
    {
        private readonly QuizDbContext _db;

        // Receives the database context through dependency injection.
        public StudyNoteRepository(QuizDbContext db)
        {
            _db = db;
        }

        // Retrieves the user's notes, with the newest notes first.
        public async Task<IEnumerable<StudyNote>> GetAll(string userId)
        {
            return await _db.StudyNotes
                .AsNoTracking()
                .Where(note => note.UserId == userId)
                .OrderByDescending(note => note.CreatedAt)
                .ToListAsync();
        }

        // Searches the user's notes by text and creation date.
        // Adapted from Microsoft Learn's ASP.NET Core MVC search tutorial:
        // https://learn.microsoft.com/aspnet/core/tutorials/first-mvc-app/search
        public async Task<IEnumerable<StudyNote>> Search(
            string userId,
            string? searchText,
            DateTime? createdFrom)
        {
            // Limits all search results to the user's own notes.
            var notes = _db.StudyNotes
                .AsNoTracking()
                .Where(note => note.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                notes = notes.Where(note =>
                    note.Title.Contains(searchText)
                    || note.Content.Contains(searchText));
            }

            if (createdFrom.HasValue)
            {
                var fromDate = createdFrom.Value.Date;

                notes = notes.Where(note =>
                    note.CreatedAt >= fromDate);
            }

            return await notes
                .OrderByDescending(note => note.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a note only if it belongs to the specified user.
        public async Task<StudyNote?> GetById(int id, string userId)
        {
            return await _db.StudyNotes
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == id
                    && note.UserId == userId);
        }

        // Saves a new study note.
        public async Task Create(StudyNote studyNote)
        {
            _db.StudyNotes.Add(studyNote);
            await _db.SaveChangesAsync();
        }

        // Updates only the title and content of a note owned by the user.
        public async Task<bool> Update(
            StudyNote studyNote,
            string userId)
        {
            var existingNote = await _db.StudyNotes
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == studyNote.StudyNoteId
                    && note.UserId == userId);

            if (existingNote == null)
            {
                return false;
            }

            existingNote.Title = studyNote.Title;
            existingNote.Content = studyNote.Content;

            await _db.SaveChangesAsync();

            return true;
        }

        // Deletes a note only if it belongs to the specified user.
        public async Task<bool> Delete(int id, string userId)
        {
            var studyNote = await _db.StudyNotes
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == id
                    && note.UserId == userId);

            if (studyNote == null)
            {
                return false;
            }

            _db.StudyNotes.Remove(studyNote);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}