using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    // Requires a login by default. Browsing quizzes is opened with [AllowAnonymous].
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly ILogger<QuizzesController> _logger;

        public QuizzesController(
            IQuizRepository quizRepository,
            IQuizAttemptRepository quizAttemptRepository,
            ILogger<QuizzesController> logger)
        {
            _quizRepository = quizRepository;
            _quizAttemptRepository = quizAttemptRepository;
            _logger = logger;
        }

        // GET /api/quizzes?search=...&topic=...
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAll([FromQuery] string? search, [FromQuery] string? topic)
        {
            var quizzes = await _quizRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                quizzes = quizzes.Where(q =>
                    q.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    q.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(topic))
            {
                quizzes = quizzes.Where(q => q.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));
            }

            var userId = GetUserId();
            var result = new List<QuizDto>();

            foreach (var quiz in quizzes)
            {
                var latest = await _quizAttemptRepository.GetLatestAsync(quiz.QuizId, userId);
                var attemptCount = await _quizAttemptRepository.CountAsync(quiz.QuizId, userId);
                bool perfectFirstAttempt = attemptCount == 1 && latest != null && latest.Score == latest.MaxScore;

                result.Add(new QuizDto
                {
                    QuizId = quiz.QuizId,
                    Title = quiz.Title,
                    Topic = quiz.Topic,
                    Description = quiz.Description,
                    QuestionCount = quiz.QuizQuestions.Count,
                    LastAttemptId = latest?.QuizAttemptId,
                    LastScore = latest?.Score,
                    LastMaxScore = latest?.MaxScore,
                    AttemptCount = attemptCount,
                    IsSolutionUnlocked = attemptCount >= 2 || perfectFirstAttempt
                });
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetById(int id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null)
            {
                _logger.LogWarning("[QuizzesController] Quiz {Id} not found.", id);
                return NotFound();
            }

            return Ok(new QuizDto
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Topic = quiz.Topic,
                Description = quiz.Description,
                QuestionCount = quiz.QuizQuestions.Count
            });
        }

        // GET /api/quizzes/{id}/take, safe version, no correct answers.
        [HttpGet("{id}/take")]
        public async Task<ActionResult<QuizTakeDto>> GetForTaking(int id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null)
            {
                _logger.LogWarning("[QuizzesController] Quiz {Id} not found for taking.", id);
                return NotFound();
            }

            var dto = new QuizTakeDto
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Questions = quiz.QuizQuestions
                    .OrderBy(qq => qq.Order)
                    .Where(qq => qq.Question != null)
                    .Select(qq => new QuestionTakeDto
                    {
                        QuestionId = qq.Question!.QuestionId,
                        Text = qq.Question.Text,
                        AllowMultipleAnswers = qq.Question.AllowMultipleAnswers,
                        ImageUrl = qq.Question.ImageUrl,
                        AnswerOptions = qq.Question.AnswerOptions.Select(o => new AnswerOptionDto
                        {
                            AnswerOptionId = o.AnswerOptionId,
                            Text = o.Text,
                            ImageUrl = o.ImageUrl
                        }).ToList()
                    }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<QuizDto>> Create(CreateQuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var quiz = new Quiz
            {
                Title = dto.Title,
                Topic = dto.Topic,
                Description = dto.Description
            };

            var created = await _quizRepository.CreateAsync(quiz);
            if (!created)
            {
                _logger.LogError("[QuizzesController] Failed to create quiz.");
                return StatusCode(500, "Could not create quiz.");
            }

            return CreatedAtAction(nameof(GetById), new { id = quiz.QuizId }, quiz);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateQuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _quizRepository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("[QuizzesController] Quiz {Id} not found for update.", id);
                return NotFound();
            }

            existing.Title = dto.Title;
            existing.Topic = dto.Topic;
            existing.Description = dto.Description;

            var updated = await _quizRepository.UpdateAsync(existing);
            if (!updated)
            {
                _logger.LogError("[QuizzesController] Failed to update quiz {Id}.", id);
                return StatusCode(500, "Could not update quiz.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quizRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("[QuizzesController] Quiz {Id} not found for deletion.", id);
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/questions/{questionId}")]
        public async Task<IActionResult> AddQuestion(int id, int questionId, [FromQuery] int order)
        {
            var added = await _quizRepository.AddQuestionAsync(id, questionId, order);
            if (!added)
            {
                _logger.LogWarning("[QuizzesController] Could not link question {QuestionId} to quiz {QuizId}.", questionId, id);
                return BadRequest("Could not link question to quiz.");
            }
            return NoContent();
        }

        [HttpDelete("{id}/questions/{questionId}")]
        public async Task<IActionResult> RemoveQuestion(int id, int questionId)
        {
            var removed = await _quizRepository.RemoveQuestionAsync(id, questionId);
            if (!removed)
            {
                _logger.LogWarning("[QuizzesController] Could not remove question {QuestionId} from quiz {QuizId}.", questionId, id);
                return NotFound();
            }
            return NoContent();
        }

        // Reads the user's ID from the token. A visitor who is not logged in
        // gets an empty ID, which matches no attempts.
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}