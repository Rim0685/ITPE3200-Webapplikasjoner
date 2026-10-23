// This project checks database operations independently of the API.
// This file tests flashcard CRUD and filtering by owner.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using QuizApi.DAL;
using QuizApi.Models;

// Creates a temporary SQLite database in memory.
// The application's database files are not used.
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<QuizDbContext>()
    .UseSqlite(connection)
    .Options;

await using var db = new QuizDbContext(options);

// Creates tables for this temporary test database.
await db.Database.EnsureCreatedAsync();

var repository = new FlashcardRepository(
    db,
    NullLogger<FlashcardRepository>.Instance);

const string ownerId = "test-user-1";
const string otherUserId = "test-user-2";

// Stops the test if a check fails.
void Check(bool condition, string description)
{
    if (!condition)
    {
        throw new Exception($"FAIL: {description}");
    }

    Console.WriteLine($"PASS: {description}");
}

// CREATE: Saves a card belonging to the first test user.
var card = new Flashcard
{
    Front = "What does HTML stand for?",
    Back = "HyperText Markup Language",
    UserId = ownerId
};

Check(await repository.CreateAsync(card), "Create flashcard");
Check(card.FlashcardId > 0, "Generate flashcard ID");

// Clears tracked objects so reads retrieve data from the database.
db.ChangeTracker.Clear();

// READ: Checks that the saved content can be retrieved.
var savedCard = await repository.GetByIdAsync(
    card.FlashcardId, ownerId);

Check(
    savedCard != null &&
    savedCard.Front == card.Front &&
    savedCard.Back == card.Back,
    "Read saved flashcard content");

// OWNER FILTER: Another user must not retrieve the card.
Check(
    await repository.GetByIdAsync(card.FlashcardId, otherUserId) == null,
    "Hide flashcard from another user");

var otherUsersCards = await repository.GetAllAsync(otherUserId);

Check(
    !otherUsersCards.Any(),
    "Exclude another user's cards from the list");

// UPDATE: Another user must not update the card.
var changes = new Flashcard
{
    FlashcardId = card.FlashcardId,
    Front = "Updated question",
    Back = "Updated answer"
};

Check(
    !await repository.UpdateAsync(changes, otherUserId),
    "Reject update by another user");

// UPDATE: The owner can update the card.
Check(
    await repository.UpdateAsync(changes, ownerId),
    "Update flashcard as owner");

db.ChangeTracker.Clear();

var updatedCard = await repository.GetByIdAsync(
    card.FlashcardId, ownerId);

Check(
    updatedCard != null &&
    updatedCard.Front == changes.Front &&
    updatedCard.Back == changes.Back &&
    updatedCard.UserId == ownerId,
    "Save updated content and preserve owner");

// DELETE: Another user must not delete the card.
Check(
    !await repository.DeleteAsync(card.FlashcardId, otherUserId),
    "Reject deletion by another user");

// DELETE: The owner can delete the card.
Check(
    await repository.DeleteAsync(card.FlashcardId, ownerId),
    "Delete flashcard as owner");

db.ChangeTracker.Clear();

Check(
    await repository.GetByIdAsync(card.FlashcardId, ownerId) == null,
    "Confirm flashcard was deleted");

Console.WriteLine("All flashcard database checks passed.");