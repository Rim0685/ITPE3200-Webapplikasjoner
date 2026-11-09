// The Controllers folder contains classes that handle web requests.
// This controller manages study notes belonging to the authenticated user.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers
{
    [Authorize]
    public class StudyNoteController : Controller
    {
        private readonly IStudyNoteRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudyNoteController> _logger;

        // Receives repository, user management and logging services.
        public StudyNoteController(
            IStudyNoteRepository repository,
            UserManager<ApplicationUser> userManager,
            ILogger<StudyNoteController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        // Displays the user's study notes with optional search filters.
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchText,
            DateTime? createdFrom)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var model = new StudyNoteListViewModel
                {
                    SearchText = searchText,
                    CreatedFrom = createdFrom
                };

                // Displays errors if a filter could not be parsed.
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var notes = await _repository.Search(
                    userId,
                    searchText,
                    createdFrom);

                model.Notes = notes
                    .Select(note => new StudyNoteViewModel
                    {
                        StudyNoteId = note.StudyNoteId,
                        Title = note.Title,
                        Content = note.Content,
                        CreatedAt = note.CreatedAt
                    })
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search study notes.");

                return StatusCode(
                    500,
                    "The notes could not be loaded. Please try again.");
            }
        }

        // Displays a note only if it belongs to the authenticated user.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var note = await _repository.GetById(id, userId);

                if (note == null)
                {
                    return NotFound();
                }

                var model = new StudyNoteViewModel
                {
                    StudyNoteId = note.StudyNoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Failed to load study note {StudyNoteId}.", id);

                return StatusCode(
                    500,
                    "The note could not be loaded. Please try again.");
            }
        }

        // Displays an empty creation form.
        [HttpGet]
        public IActionResult Create()
        {
            return View(new StudyNoteViewModel());
        }

        // Creates a note with ownership and creation time set on the server.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudyNoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var note = new StudyNote
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.Create(note);

                _logger.LogInformation(
                    "Created study note {StudyNoteId}.",
                    note.StudyNoteId);

                return RedirectToAction(
                    nameof(Details),
                    new { id = note.StudyNoteId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a study note.");

                ModelState.AddModelError(
                    string.Empty,
                    "The note could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays the edit form for a note owned by the user.
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var note = await _repository.GetById(id, userId);

                if (note == null)
                {
                    return NotFound();
                }

                var model = new StudyNoteViewModel
                {
                    StudyNoteId = note.StudyNoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load study note {StudyNoteId} for editing.",
                    id);

                return StatusCode(
                    500,
                    "The note could not be loaded. Please try again.");
            }
        }

        // Updates the title and content after checking ownership.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(StudyNoteViewModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var note = await _repository.GetById(
                    model.StudyNoteId,
                    userId);

                if (note == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    model.CreatedAt = note.CreatedAt;
                    return View(model);
                }

                // The repository preserves ownership and creation time.
                bool updated = await _repository.Update(
                    new StudyNote
                    {
                        StudyNoteId = model.StudyNoteId,
                        Title = model.Title,
                        Content = model.Content
                    },
                    userId);

                if (!updated)
                {
                    return NotFound();
                }

                _logger.LogInformation(
                    "Updated study note {StudyNoteId}.",
                    model.StudyNoteId);

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.StudyNoteId });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update study note {StudyNoteId}.",
                    model.StudyNoteId);

                ModelState.AddModelError(
                    string.Empty,
                    "The changes could not be saved. Please try again.");

                return View(model);
            }
        }

        // Displays a confirmation page before deleting a note.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                var note = await _repository.GetById(id, userId);

                if (note == null)
                {
                    return NotFound();
                }

                var model = new StudyNoteViewModel
                {
                    StudyNoteId = note.StudyNoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load study note {StudyNoteId} for deletion.",
                    id);

                return StatusCode(
                    500,
                    "The note could not be loaded. Please try again.");
            }
        }

        // Deletes a note only after verifying its ownership.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Challenge();
                }

                bool deleted = await _repository.Delete(id, userId);

                if (!deleted)
                {
                    return NotFound();
                }

                _logger.LogInformation(
                    "Deleted study note {StudyNoteId}.",
                    id);

                TempData["SuccessMessage"] =
                    "The note has been deleted.";

                return RedirectToAction("Index", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete study note {StudyNoteId}.",
                    id);

                return StatusCode(
                    500,
                    "The note could not be deleted. Please try again.");
            }
        }
    }
}