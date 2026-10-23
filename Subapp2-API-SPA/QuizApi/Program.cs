// Program.cs configures services and the HTTP request pipeline.

using Microsoft.EntityFrameworkCore;
using QuizApi.DAL;

var builder = WebApplication.CreateBuilder(args);

// Registers OpenAPI document generation.
builder.Services.AddOpenApi();

// Registers the quiz repository for dependency injection.
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Registers the question repository for dependency injection.
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Registers the flashcard repository for dependency injection.
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();

// Registers the quiz attempt repository for dependency injection.
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();

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

app.Run();