// The Controllers folder contains the API's HTTP endpoints.
// This controller lets a logged-in user create, read, update and delete
// their own study notes. Study notes have no extra business rules,
// so the controller uses the repository directly without a service class.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.DAL;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    // [Authorize] returns 401 for every endpoint here if the token is missing or invalid.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StudyNotesController : ControllerBase
    {
        private readonly IStudyNoteRepository _noteRepository;
        private readonly ILogger<StudyNotesController> _logger;

        public StudyNotesController(
            IStudyNoteRepository noteRepository,
            ILogger<StudyNotesController> logger)
        {
            _noteRepository = noteRepository;
            _logger = logger;
        }

        // GET /api/studynotes: returns the user's own notes.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudyNoteDto>>> GetAll()
        {
            try
            {
                var notes = await _noteRepository.GetAllAsync(GetUserId());

                return Ok(notes.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNotesController] Failed to load study notes.");
                return StatusCode(500, "Could not load the study notes.");
            }
        }

        // GET /api/studynotes/{id}: another user's note counts as not found.
        [HttpGet("{id}")]
        public async Task<ActionResult<StudyNoteDto>> GetById(int id)
        {
            try
            {
                var note = await _noteRepository.GetByIdAsync(id, GetUserId());

                if (note == null)
                {
                    return NotFound("The study note was not found.");
                }

                return Ok(MapToDto(note));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNotesController] Failed to load note {Id}.", id);
                return StatusCode(500, "Could not load the study note.");
            }
        }

        // POST /api/studynotes: creates a note owned by the logged-in user.
        // [ApiController] returns 400 automatically if the DTO validation fails.
        [HttpPost]
        public async Task<ActionResult<StudyNoteDto>> Create(SaveStudyNoteDto dto)
        {
            try
            {
                // The owner comes from the token, never from the client's data.
                var note = new StudyNote
                {
                    Title = dto.Title.Trim(),
                    Content = dto.Content.Trim(),
                    UserId = GetUserId()
                };

                if (!await _noteRepository.CreateAsync(note))
                {
                    return StatusCode(500, "Could not create the study note.");
                }

                return CreatedAtAction(nameof(GetById), new { id = note.StudyNoteId }, MapToDto(note));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNotesController] Failed to create a study note.");
                return StatusCode(500, "Could not create the study note.");
            }
        }

        // PUT /api/studynotes/{id}: updates one of the user's own notes.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SaveStudyNoteDto dto)
        {
            try
            {
                var userId = GetUserId();

                // Checked first so "not found" (404) is separated from "failed" (500).
                if (await _noteRepository.GetByIdAsync(id, userId) == null)
                {
                    return NotFound("The study note was not found.");
                }

                var note = new StudyNote
                {
                    StudyNoteId = id,
                    Title = dto.Title.Trim(),
                    Content = dto.Content.Trim()
                };

                if (!await _noteRepository.UpdateAsync(note, userId))
                {
                    return StatusCode(500, "Could not update the study note.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNotesController] Failed to update note {Id}.", id);
                return StatusCode(500, "Could not update the study note.");
            }
        }

        // DELETE /api/studynotes/{id}: deletes one of the user's own notes.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();

                if (await _noteRepository.GetByIdAsync(id, userId) == null)
                {
                    return NotFound("The study note was not found.");
                }

                if (!await _noteRepository.DeleteAsync(id, userId))
                {
                    return StatusCode(500, "Could not delete the study note.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudyNotesController] Failed to delete note {Id}.", id);
                return StatusCode(500, "Could not delete the study note.");
            }
        }

        // Copies only the fields the client is allowed to see.
        private static StudyNoteDto MapToDto(StudyNote note)
        {
            return new StudyNoteDto
            {
                StudyNoteId = note.StudyNoteId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt
            };
        }

        // Reads the user's ID from the token. [Authorize] guarantees that a user exists.
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
