using Microsoft.AspNetCore.Mvc;
using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(IQuestionRepository questionRepository, ILogger<QuestionsController> logger)
        {
            _questionRepository = questionRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
        {
            var questions = await _questionRepository.GetAllAsync();
            return Ok(questions.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                _logger.LogWarning("[QuestionsController] Question {Id} not found.", id);
                return NotFound();
            }
            return Ok(MapToDto(question));
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var question = new Question
            {
                Text = dto.Text,
                Explanation = dto.Explanation,
                AllowMultipleAnswers = dto.AllowMultipleAnswers,
                ImageUrl = dto.ImageUrl,
                AnswerOptions = dto.AnswerOptions.Select(o => new AnswerOption
                {
                    Text = o.Text,
                    ImageUrl = o.ImageUrl,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            var created = await _questionRepository.CreateAsync(question);
            if (!created)
            {
                _logger.LogError("[QuestionsController] Failed to create question.");
                return StatusCode(500, "Could not create question.");
            }

            return CreatedAtAction(nameof(GetById), new { id = question.QuestionId }, MapToDto(question));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("[QuestionsController] Question {Id} not found for update.", id);
                return NotFound();
            }

            existing.Text = dto.Text;
            existing.Explanation = dto.Explanation;
            existing.AllowMultipleAnswers = dto.AllowMultipleAnswers;
            existing.ImageUrl = dto.ImageUrl;
            existing.AnswerOptions = dto.AnswerOptions.Select(o => new AnswerOption
            {
                AnswerOptionId = o.AnswerOptionId ?? 0,
                Text = o.Text,
                ImageUrl = o.ImageUrl,
                IsCorrect = o.IsCorrect
            }).ToList();

            var updated = await _questionRepository.UpdateAsync(existing);
            if (!updated)
            {
                _logger.LogError("[QuestionsController] Failed to update question {Id}.", id);
                return StatusCode(500, "Could not update question.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("[QuestionsController] Question {Id} not found for deletion.", id);
                return NotFound();
            }
            return NoContent();
        }

        private static QuestionDto MapToDto(Question q) => new()
        {
            QuestionId = q.QuestionId,
            Text = q.Text,
            Explanation = q.Explanation,
            AllowMultipleAnswers = q.AllowMultipleAnswers,
            ImageUrl = q.ImageUrl,
            AnswerOptions = q.AnswerOptions.Select(o => new AnswerOptionAdminDto
            {
                AnswerOptionId = o.AnswerOptionId,
                Text = o.Text,
                ImageUrl = o.ImageUrl,
                IsCorrect = o.IsCorrect
            }).ToList()
        };
    }
}