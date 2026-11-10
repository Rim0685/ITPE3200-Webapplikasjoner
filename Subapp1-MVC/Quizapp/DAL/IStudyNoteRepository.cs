using Quizapp.Models;

namespace Quizapp.DAL;

public interface IStudyNoteRepository
{
    Task<IEnumerable<StudyNote>?> GetAll(string userId);

    Task<IEnumerable<StudyNote>?> Search(
        string userId, string? searchText, DateTime? createdFrom);

    Task<StudyNote?> GetById(int id, string userId);

    Task<bool> Create(StudyNote studyNote);

    Task<bool> Update(StudyNote studyNote, string userId);

    Task<bool> Delete(int id, string userId);
}