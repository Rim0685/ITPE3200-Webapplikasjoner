using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizapp.DAL;
using Quizapp.Models;
using Quizapp.ViewModels;

namespace Quizapp.Controllers;

[Authorize]
public class StudyNoteController : Controller
{
    private readonly IStudyNoteRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<StudyNoteController> _logger;

    public StudyNoteController(
        IStudyNoteRepository repository,
        UserManager<ApplicationUser> userManager,
        ILogger<StudyNoteController> logger)
    {
        _repository = repository;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchText, DateTime? createdFrom)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var model = new StudyNoteListViewModel
        {
            SearchText = searchText,
            CreatedFrom = createdFrom
        };

        if (!ModelState.IsValid)
            return View(model);

        var notes = await _repository.Search(
            userId, searchText, createdFrom);

        if (notes == null)
        {
            _logger.LogError("Study note search failed.");
            return StatusCode(500, "The notes could not be loaded.");
        }

        model.Notes = notes.Select(note => new StudyNoteViewModel
        {
            StudyNoteId = note.StudyNoteId,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var note = await _repository.GetById(id, userId);
        if (note == null)
        {
            _logger.LogWarning(
                "Study note {StudyNoteId} was not available.", id);

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

    [HttpGet]
    public IActionResult Create()
    {
        return View(new StudyNoteViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudyNoteViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        if (ModelState.IsValid)
        {
            var note = new StudyNote
            {
                Title = model.Title,
                Content = model.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            bool returnOk = await _repository.Create(note);
            if (returnOk)
                return RedirectToAction(
                    nameof(Details), new { id = note.StudyNoteId });

            ModelState.AddModelError(
                string.Empty, "The note could not be saved.");
        }

        _logger.LogWarning("Study note creation failed.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var note = await _repository.GetById(id, userId);
        if (note == null)
        {
            _logger.LogWarning(
                "Study note {StudyNoteId} was not available for editing.", id);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(StudyNoteViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var note = await _repository.GetById(model.StudyNoteId, userId);
        if (note == null)
            return NotFound();

        // Restores the stored date when redisplaying the form.
        model.CreatedAt = note.CreatedAt;

        if (ModelState.IsValid)
        {
            note.Title = model.Title;
            note.Content = model.Content;

            bool returnOk = await _repository.Update(note, userId);
            if (returnOk)
                return RedirectToAction(
                    nameof(Details), new { id = note.StudyNoteId });

            ModelState.AddModelError(
                string.Empty, "The changes could not be saved.");
        }

        _logger.LogWarning(
            "Study note update failed for {StudyNoteId}.",
            model.StudyNoteId);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var note = await _repository.GetById(id, userId);
        if (note == null)
        {
            _logger.LogWarning(
                "Study note {StudyNoteId} was not available for deletion.", id);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        bool returnOk = await _repository.Delete(id, userId);
        if (!returnOk)
        {
            _logger.LogWarning(
                "Study note deletion failed for {StudyNoteId}.", id);

            return BadRequest("The note could not be deleted.");
        }

        TempData["SuccessMessage"] = "The note has been deleted.";
        return RedirectToAction("Index", "Profile");
    }
}