// The Controllers folder contains the API's HTTP endpoints.
// This controller handles user registration and login, and issues JWT tokens.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Uses one message for all failed logins, so the response does not reveal
        // which email addresses are registered or which accounts are locked.
        private const string InvalidLoginMessage =
            "Invalid email or password. An account is locked for 5 minutes after 5 failed attempts.";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        // POST /api/auth/register: creates a user and logs them in directly.
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            try
            {
                // The email is also the username, so Identity keeps it unique.
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(
                        "[AuthController] Registration rejected: {ErrorCodes}.",
                        result.Errors.Select(e => e.Code));

                    // Returns Identity's messages in the same format as other validation errors.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return ValidationProblem(ModelState);
                }

                _logger.LogInformation(
                    "[AuthController] Registered user {UserId}.", user.Id);

                return Ok(CreateAuthResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController] Failed to register a user.");
                return StatusCode(500, "Could not complete the registration.");
            }
        }

        // POST /api/auth/login: checks the credentials and returns a token.
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    _logger.LogWarning("[AuthController] Failed login attempt.");
                    return Unauthorized(InvalidLoginMessage);
                }

                // A locked account is rejected even if the password is correct.
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning(
                        "[AuthController] Login attempt on locked user {UserId}.", user.Id);

                    return Unauthorized(InvalidLoginMessage);
                }

                if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    // Counts the failed attempt, and locks the account at the limit.
                    await _userManager.AccessFailedAsync(user);

                    _logger.LogWarning(
                        "[AuthController] Wrong password for user {UserId}.", user.Id);

                    return Unauthorized(InvalidLoginMessage);
                }

                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation(
                    "[AuthController] User {UserId} logged in.", user.Id);

                return Ok(CreateAuthResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController] Failed to process a login.");
                return StatusCode(500, "Could not complete the login.");
            }
        }

        // Creates a signed token that identifies the user on later requests.
        private AuthResponseDto CreateAuthResponse(ApplicationUser user)
        {
            var email = user.Email ?? string.Empty;

            // NameIdentifier holds the user's ID, which links the user to their data.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var expiresAt = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue("Jwt:ExpiresInMinutes", 60));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256));

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = email,
                ExpiresAt = expiresAt
            };
        }
    }
}
