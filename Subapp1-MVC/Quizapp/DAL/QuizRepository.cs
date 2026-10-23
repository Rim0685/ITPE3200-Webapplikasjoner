using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Performs the database operations defined in IQuizRepository.
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizDbContext _db;

        public QuizRepository(QuizDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Quiz>> GetAll()
        {
            return await _db.Quizzes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizById(int id)
        {
            return await _db.Quizzes
                .Include(quiz => quiz.Questions)
                    .ThenInclude(question => question.AnswerOptions)
                .FirstOrDefaultAsync(quiz => quiz.QuizId == id);
        }

        public async Task Create(Quiz quiz)
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Quiz quiz)
        {
            _db.Quizzes.Update(quiz);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);

            if (quiz == null)
            {
                return false;
            }

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task AddQuizAttempt(QuizAttempt quizAttempt)
        {
            _db.QuizAttempts.Add(quizAttempt);
            await _db.SaveChangesAsync();
        }
        
        // Retrieves a quiz attempt along with all associated answers, questions, and answer options, 
        // used by the results and answer key pages.
        public async Task<QuizAttempt?> GetQuizAttemptById(int id)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.Quiz)
                .Include(attempt => attempt.AttemptAnswers)
                    .ThenInclude(attemptAnswer => attemptAnswer.AnswerOption)
                        .ThenInclude(answerOption => answerOption!.Question)
                            .ThenInclude(question => question!.AnswerOptions)
                .FirstOrDefaultAsync(attempt => attempt.QuizAttemptId == id);
        }

        public async Task<QuizAttempt?> GetLatestQuizAttempt(int quizId)
        {
            return await _db.QuizAttempts
                .Include(attempt => attempt.AttemptAnswers)
                .Where(attempt => attempt.QuizId == quizId)
                .OrderByDescending(attempt => attempt.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetQuizAttemptCount(int quizId)
        {
            return await _db.QuizAttempts
                .CountAsync(attempt => attempt.QuizId == quizId);
        }
    }
}