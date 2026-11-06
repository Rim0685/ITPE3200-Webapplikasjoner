// The Controllers folder contains the API's HTTP endpoints.
// This controller lets a logged-in user manage their own flashcards.
// The rules are in FlashcardService, and this class chooses the HTTP response.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.DTOs;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    // [Authorize] returns 401 for every endpoint here if the token is missing or invalid.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FlashcardsController : ControllerBase
    {
        private readonly IFlashcardService _flashcardService;
        private readonly ILogger<FlashcardsController> _logger;

        public FlashcardsController(
            IFlashcardService flashcardService,
            ILogger<FlashcardsController> logger)
        {
            _flashcardService = flashcardService;
            _logger = logger;
        }

        // GET /api/flashcards: returns the user's own flashcards.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlashcardDto>>> GetAll()
        {
            try
            {
                return Ok(await _flashcardService.GetAllAsync(GetUserId()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardsController] Failed to load flashcards.");
                return StatusCode(500, "Could not load the flashcards.");
            }
        }

        // GET /api/flashcards/{id}: another user's flashcard counts as not found,
        // so the response does not reveal that the ID exists.
        [HttpGet("{id}")]
        public async Task<ActionResult<FlashcardDto>> GetById(int id)
        {
            try
            {
                var flashcard = await _flashcardService.GetByIdAsync(id, GetUserId());

                if (flashcard == null)
                {
                    return NotFound("The flashcard was not found.");
                }

                return Ok(flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardsController] Failed to load flashcard {Id}.", id);
                return StatusCode(500, "Could not load the flashcard.");
            }
        }

        // POST /api/flashcards: creates a flashcard owned by the logged-in user.
        [HttpPost]
        public async Task<ActionResult<FlashcardDto>> Create(CreateFlashcardDto dto)
        {
            try
            {
                var result = await _flashcardService.CreateAsync(dto, GetUserId());

                if (result.Status == FlashcardResultStatus.LimitReached)
                {
                    return BadRequest(
                        $"You can have at most {FlashcardService.MaxFlashcardsPerUser} flashcards. Delete one to add a new one.");
                }

                if (result.Status != FlashcardResultStatus.Success)
                {
                    return StatusCode(500, "Could not create the flashcard.");
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Flashcard!.FlashcardId },
                    result.Flashcard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardsController] Failed to create a flashcard.");
                return StatusCode(500, "Could not create the flashcard.");
            }
        }

        // PUT /api/flashcards/{id}: updates one of the user's own flashcards.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFlashcardDto dto)
        {
            try
            {
                var result = await _flashcardService.UpdateAsync(id, dto, GetUserId());

                return ToResponse(result, "Could not update the flashcard.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardsController] Failed to update flashcard {Id}.", id);
                return StatusCode(500, "Could not update the flashcard.");
            }
        }

        // DELETE /api/flashcards/{id}: deletes one of the user's own flashcards.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _flashcardService.DeleteAsync(id, GetUserId());

                return ToResponse(result, "Could not delete the flashcard.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardsController] Failed to delete flashcard {Id}.", id);
                return StatusCode(500, "Could not delete the flashcard.");
            }
        }

        // Converts the result of an update or delete to an HTTP response.
        private IActionResult ToResponse(FlashcardResult result, string failureMessage)
        {
            return result.Status switch
            {
                FlashcardResultStatus.Success => NoContent(),
                FlashcardResultStatus.NotFound => NotFound("The flashcard was not found."),
                _ => StatusCode(500, failureMessage)
            };
        }

        // Reads the user's ID from the token. [Authorize] guarantees that a user exists.
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
