// The Controllers folder contains the API's HTTP endpoints.
// This controller lets a logged-in user view, change and delete their own account.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    // The user always comes from the token, so nobody can reach another user's account.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private const string AccountNotFoundMessage = "The account was not found.";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET /api/profile: returns the logged-in user's profile.
        [HttpGet]
        public async Task<ActionResult<ProfileDto>> Get()
        {
            try
            {
                var user = await GetCurrentUserAsync();

                // A token stays valid for a while after its account has been deleted.
                if (user == null)
                {
                    return NotFound(AccountNotFoundMessage);
                }

                return Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProfileController] Failed to load a profile.");
                return StatusCode(500, "Could not load the profile.");
            }
        }

        // PUT /api/profile: changes the user's name and returns the updated profile.
        [HttpPut]
        public async Task<ActionResult<ProfileDto>> Update(UpdateProfileDto dto)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                if (user == null)
                {
                    return NotFound(AccountNotFoundMessage);
                }

                user.FirstName = dto.FirstName.Trim();
                user.LastName = dto.LastName.Trim();

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        "[ProfileController] Could not update user {UserId}: {ErrorCodes}.",
                        user.Id,
                        result.Errors.Select(e => e.Code));

                    return StatusCode(500, "Could not update the profile.");
                }

                _logger.LogInformation(
                    "[ProfileController] Updated the profile of user {UserId}.", user.Id);

                return Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProfileController] Failed to update a profile.");
                return StatusCode(500, "Could not update the profile.");
            }
        }

        // DELETE /api/profile: deletes the account after the password is confirmed.
        // The database also deletes the user's flashcards and quiz attempts.
        // [FromBody] is required because a DELETE does not read its body by default.
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteAccountDto dto)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                if (user == null)
                {
                    return NotFound(AccountNotFoundMessage);
                }

                // Returns 400 instead of 401, because the token itself is still valid
                // and the client should not log the user out over a mistyped password.
                if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    _logger.LogWarning(
                        "[ProfileController] Wrong password when deleting user {UserId}.",
                        user.Id);

                    return BadRequest("The password is incorrect.");
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        "[ProfileController] Could not delete user {UserId}: {ErrorCodes}.",
                        user.Id,
                        result.Errors.Select(e => e.Code));

                    return StatusCode(500, "Could not delete the account.");
                }

                _logger.LogInformation("[ProfileController] Deleted user {UserId}.", user.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProfileController] Failed to delete an account.");
                return StatusCode(500, "Could not delete the account.");
            }
        }

        // Finds the logged-in user from the ID in the token.
        private Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return _userManager.FindByIdAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // Copies only the fields the client is allowed to see.
        private static ProfileDto MapToDto(ApplicationUser user)
        {
            return new ProfileDto
            {
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
