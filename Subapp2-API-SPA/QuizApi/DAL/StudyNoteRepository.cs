// The DAL folder contains database configuration and data access code.
// This class handles database operations for a user's study notes.
// Every query filters on userId, so a user can never reach another user's notes.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public class StudyNoteRepository : IStudyNoteRepository
    {
        private readonly QuizDbContext _db;
        private readonly ILogger<StudyNoteRepository> _logger;

        public StudyNoteRepository(QuizDbContext db, ILogger<StudyNoteRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves the user's notes, newest first.
        public async Task<IEnumerable<StudyNote>> GetAllAsync(string userId)
        {
            try
            {
                return await _db.StudyNotes
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNoteRepository] Failed to retrieve study notes.");
                throw;
            }
        }

        // Retrieves a note only if it belongs to the specified user.
        public async Task<StudyNote?> GetByIdAsync(int noteId, string userId)
        {
            try
            {
                return await _db.StudyNotes
                    .FirstOrDefaultAsync(n => n.StudyNoteId == noteId && n.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNoteRepository] Failed to retrieve note {NoteId}.", noteId);
                throw;
            }
        }

        public async Task<bool> CreateAsync(StudyNote note)
        {
            try
            {
                _db.StudyNotes.Add(note);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "[StudyNoteRepository] Failed to create a study note.");
                return false;
            }
        }

        // Updates title and content without changing the owner or creation time.
        public async Task<bool> UpdateAsync(StudyNote note, string userId)
        {
            try
            {
                var existing = await _db.StudyNotes
                    .FirstOrDefaultAsync(n => n.StudyNoteId == note.StudyNoteId && n.UserId == userId);

                if (existing == null)
                {
                    _logger.LogWarning(
                        "[StudyNoteRepository] Note {NoteId} was not found for update.",
                        note.StudyNoteId);

                    return false;
                }

                existing.Title = note.Title;
                existing.Content = note.Content;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "[StudyNoteRepository] Failed to update note {NoteId}.", note.StudyNoteId);
                return false;
            }
        }

        // Deletes a note only if the user owns it.
        public async Task<bool> DeleteAsync(int noteId, string userId)
        {
            try
            {
                var note = await _db.StudyNotes
                    .FirstOrDefaultAsync(n => n.StudyNoteId == noteId && n.UserId == userId);

                if (note == null)
                {
                    _logger.LogWarning(
                        "[StudyNoteRepository] Note {NoteId} was not found for deletion.", noteId);

                    return false;
                }

                _db.StudyNotes.Remove(note);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "[StudyNoteRepository] Failed to delete note {NoteId}.", noteId);
                return false;
            }
        }
    }
}
