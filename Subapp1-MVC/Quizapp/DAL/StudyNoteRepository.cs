using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL;

public class StudyNoteRepository : IStudyNoteRepository
{
    private readonly QuizDbContext _db;
    private readonly ILogger<StudyNoteRepository> _logger;

    public StudyNoteRepository(
        QuizDbContext db,
        ILogger<StudyNoteRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<StudyNote>?> GetAll(string userId)
    {
        try
        {
            return await _db.StudyNotes
                .AsNoTracking()
                .Where(note => note.UserId == userId)
                .OrderByDescending(note => note.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load study notes.");
            return null;
        }
    }

    // Filters only the authenticated user's notes.
    public async Task<IEnumerable<StudyNote>?> Search(
        string userId,
        string? searchText,
        DateTime? createdFrom)
    {
        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search study notes.");
            return null;
        }
    }

    public async Task<StudyNote?> GetById(int id, string userId)
    {
        try
        {
            return await _db.StudyNotes
                .AsNoTracking()
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == id
                    && note.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to load study note {StudyNoteId}.", id);

            return null;
        }
    }

    public async Task<bool> Create(StudyNote studyNote)
    {
        try
        {
            _db.StudyNotes.Add(studyNote);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create a study note.");
            return false;
        }
    }

    public async Task<bool> Update(
        StudyNote studyNote, string userId)
    {
        try
        {
            var existingNote = await _db.StudyNotes
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == studyNote.StudyNoteId
                    && note.UserId == userId);

            if (existingNote == null)
            {
                return false;
            }

            // Preserves the owner and creation date.
            existingNote.Title = studyNote.Title;
            existingNote.Content = studyNote.Content;

            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to update study note {StudyNoteId}.",
                studyNote.StudyNoteId);

            return false;
        }
    }

    public async Task<bool> Delete(int id, string userId)
    {
        try
        {
            var note = await _db.StudyNotes
                .FirstOrDefaultAsync(note =>
                    note.StudyNoteId == id
                    && note.UserId == userId);

            if (note == null)
            {
                return false;
            }

            _db.StudyNotes.Remove(note);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to delete study note {StudyNoteId}.", id);

            return false;
        }
    }
}