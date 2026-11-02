using Microsoft.AspNetCore.Mvc;
using QuizApi.DTOs;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAttemptsController : ControllerBase
    {
        private readonly IQuizEvaluationService _evaluationService;
        private readonly ILogger<QuizAttemptsController> _logger;

        public QuizAttemptsController(IQuizEvaluationService evaluationService, ILogger<QuizAttemptsController> logger)
        {
            _evaluationService = evaluationService;
            _logger = logger;
        }

        // POST /api/quizattempts — submits answers, returns score (no correct answers)
        [HttpPost]
        public async Task<ActionResult<QuizResultDto>> Submit(SubmitQuizDto submission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _evaluationService.EvaluateAndSaveAsync(submission, userId);

            if (result == null)
            {
                _logger.LogError("[QuizAttemptsController] Failed to evaluate submission for QuizId {QuizId}.", submission.QuizId);
                return StatusCode(500, "Could not process quiz submission.");
            }

            return Ok(result);
        }

        // GET /api/quizattempts/{id}/solution — only returns data once unlocked
        [HttpGet("{id}/solution")]
        public async Task<ActionResult<QuizSolutionDto>> GetSolution(int id)
        {
            var userId = GetUserId();
            var solution = await _evaluationService.GetSolutionAsync(id, userId);

            if (solution == null)
            {
                _logger.LogWarning("[QuizAttemptsController] Solution not available for QuizAttempt {Id}, UserId {UserId}.", id, userId);
                return StatusCode(StatusCodes.Status403Forbidden, "Solution is not unlocked yet.");
            }

            return Ok(solution);
        }

        // Placeholder !!!
        private string GetUserId() => User.Identity?.Name ?? "demo-user";
    }
}