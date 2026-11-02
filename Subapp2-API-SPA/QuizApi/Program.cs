// Program.cs configures services and the HTTP request pipeline.

using Microsoft.EntityFrameworkCore;
using QuizApi.DAL;
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

// Routes incoming requests to your controllers (QuizzesController, etc.).
app.MapControllers();

app.Run();

app.UseHttpsRedirection();

app.Run();