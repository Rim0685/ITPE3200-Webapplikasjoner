// The Controllers folder contains the API's HTTP endpoints.
// This controller lets a logged-in user manage their own flashcard decks.
// The rules are in FlashcardDeckService, and this class chooses the HTTP response.

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
    public class FlashcardDecksController : ControllerBase
    {
        private readonly IFlashcardDeckService _deckService;
        private readonly ILogger<FlashcardDecksController> _logger;

        public FlashcardDecksController(
            IFlashcardDeckService deckService,
            ILogger<FlashcardDecksController> logger)
        {
            _deckService = deckService;
            _logger = logger;
        }

        // GET /api/flashcarddecks: returns the user's own decks.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlashcardDeckDto>>> GetAll()
        {
            try
            {
                return Ok(await _deckService.GetAllAsync(GetUserId()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardDecksController] Failed to load decks.");
                return StatusCode(500, "Could not load the decks.");
            }
        }

        // GET /api/flashcarddecks/{id}: another user's deck counts as not found.
        [HttpGet("{id}")]
        public async Task<ActionResult<FlashcardDeckDto>> GetById(int id)
        {
            try
            {
                var deck = await _deckService.GetByIdAsync(id, GetUserId());

                if (deck == null)
                {
                    return NotFound("The deck was not found.");
                }

                return Ok(deck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardDecksController] Failed to load deck {Id}.", id);
                return StatusCode(500, "Could not load the deck.");
            }
        }

        // POST /api/flashcarddecks: creates a deck owned by the logged-in user.
        [HttpPost]
        public async Task<ActionResult<FlashcardDeckDto>> Create(CreateFlashcardDeckDto dto)
        {
            try
            {
                var result = await _deckService.CreateAsync(dto, GetUserId());

                if (result.Status == FlashcardDeckResultStatus.Success)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Deck!.FlashcardDeckId },
                        result.Deck);
                }

                return (ActionResult)ToResponse(result, "Could not create the deck.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardDecksController] Failed to create a deck.");
                return StatusCode(500, "Could not create the deck.");
            }
        }

        // PUT /api/flashcarddecks/{id}: updates one of the user's own decks.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFlashcardDeckDto dto)
        {
            try
            {
                var result = await _deckService.UpdateAsync(id, dto, GetUserId());

                return ToResponse(result, "Could not update the deck.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardDecksController] Failed to update deck {Id}.", id);
                return StatusCode(500, "Could not update the deck.");
            }
        }

        // DELETE /api/flashcarddecks/{id}: deletes the deck and the flashcards in it.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _deckService.DeleteAsync(id, GetUserId());

                return ToResponse(result, "Could not delete the deck.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FlashcardDecksController] Failed to delete deck {Id}.", id);
                return StatusCode(500, "Could not delete the deck.");
            }
        }

        // Converts the result of an operation to an HTTP response.
        private IActionResult ToResponse(FlashcardDeckResult result, string failureMessage)
        {
            return result.Status switch
            {
                FlashcardDeckResultStatus.Success => NoContent(),
                FlashcardDeckResultStatus.NotFound => NotFound("The deck was not found."),
                FlashcardDeckResultStatus.LimitReached => BadRequest(
                    $"You can have at most {FlashcardDeckService.MaxDecksPerUser} decks. Delete one to add a new one."),
                FlashcardDeckResultStatus.DuplicateName => Conflict("You already have a deck with this name."),
                _ => StatusCode(500, failureMessage)
            };
        }

        // Reads the user's ID from the token. [Authorize] guarantees that a user exists.
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}