// Program.cs configures services and the HTTP request pipeline.

using Microsoft.EntityFrameworkCore;
using QuizApi.DAL;

var builder = WebApplication.CreateBuilder(args);

// Registers OpenAPI document generation.
builder.Services.AddOpenApi();

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