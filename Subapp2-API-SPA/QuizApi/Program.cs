// Program.cs configures services and the HTTP request pipeline.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuizApi.DAL;
using QuizApi.Models;
using QuizApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Registers controller support (needed for [ApiController]-based endpoints).
builder.Services.AddControllers();

// Registers OpenAPI document generation.
builder.Services.AddOpenApi();

// Allows the React SPA (running on a different port) to call this API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpa", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite's default dev port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registers the quiz repository for dependency injection.
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Registers the question repository for dependency injection.
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Registers the flashcard repository for dependency injection.
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();

// Registers the quiz attempt repository for dependency injection.
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();

// Registers the quiz evaluation service (scoring and fasit-unlock logic).
builder.Services.AddScoped<IQuizEvaluationService, QuizEvaluationService>();

// Registers the database context with an SQLite connection.
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("QuizDbContext")
        ?? throw new InvalidOperationException(
            "Connection string 'QuizDbContext' was not found.")));

// Registers ASP.NET Core Identity, which stores users and hashes their passwords.
// AddIdentityCore is used because the API authenticates with JWT, not cookies.
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<QuizDbContext>();

// Reads the signing key, and stops the application at startup if it is missing.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configuration value 'Jwt:Key' was not found.");

// Registers JWT authentication and defines which tokens the API accepts.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Registers the policies used by [Authorize].
builder.Services.AddAuthorization();

var app = builder.Build();

// Makes the OpenAPI document available during development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Adds example data during development.
    await DBInit.SeedAsync(app);
}

app.UseHttpsRedirection();

// Must come before MapControllers so cross-origin requests from the SPA are allowed.
app.UseCors("AllowSpa");

// Identifies the user from the token. Must come before authorization.
app.UseAuthentication();

// Enforces [Authorize] on the endpoints.
app.UseAuthorization();

// Routes incoming requests to your controllers (QuizzesController, etc.).
app.MapControllers();

app.Run();