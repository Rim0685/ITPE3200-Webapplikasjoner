using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Quizapp.DAL;
using Quizapp.Models;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Quizapp.Controllers;

namespace Quizapp.Tests;
public class PositiveTests  
{
    // Test 1: Checks that an existing quiz can be retrieved by ID
    [Fact]
    public async Task GetQuizById_ExistingQuiz_ReturnsQuiz()
    {
        // Arrange: Set up the test database, repository and quiz
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new QuizDbContext(options);

        var repository = new QuizRepository(
            db, NullLogger<QuizRepository>.Instance);
        
        var quiz = new Quiz
        {
            Title = "Test Quiz",
        };

        db.Quizzes.Add(quiz);
        await db.SaveChangesAsync();

        // Act: Retrieve the quiz by its ID.
        var result = await repository.GetQuizById(quiz.QuizId);

        // Assert: Verifies that the correct quiz was returned.
        Assert.NotNull(result);
        Assert.Equal("Test Quiz", result.Title);
    }

    // Test 2: Checks that two correct answers give a score of two.
    [Fact]
    public async Task Submit_ValidAnswers_CalculatesCorrectScore()
    {
        // Arrange: Create a quiz with three questions.
        var quiz = new Quiz
        {
            QuizId = 1,
            Title = "Test Quiz",
            Questions = new List<Question>()
        };

        for (int i = 1; i <= 3; i++)
        {
            quiz.Questions.Add(new Question
            {

                QuestionId = i,
                QuestionText = $"Question {i}",
                QuestionType = QuestionType.TextSingle,
                AnswerOptions = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        AnswerOptionId = i * 10,
                        IsCorrect = true
                    },
                    new AnswerOption
                    {
                        AnswerOptionId = i * 10 + 1,
                        IsCorrect = false
                    }
                }
            });
        }

        var savedAttempt = (QuizAttempt?)null;

        var repository = new Mock<IQuizRepository>();

        repository.Setup(r => r.GetQuizById(1))
            .ReturnsAsync(quiz);

        repository.Setup(r => r.AddQuizAttempt(It.IsAny<QuizAttempt>()))
            .Callback<QuizAttempt>(attempt => savedAttempt = attempt)
            .ReturnsAsync(true);

        // Arrange: Set up a test user.
        var userStore = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new UserManager<ApplicationUser>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var controller = new ResultController(
            repository.Object,
            NullLogger<ResultController>.Instance,
            userManager);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test-user")
                    }, "TestAuhentication"))
            }
       };

       var submission = new Quizapp.ViewModels.SubmitQuizViewModel
       {
           QuizId = 1,
           Answers = new List<Quizapp.ViewModels.SubmitAnswerViewModel>
           {
               new() { QuestionId = 1, SelectedAnswerOptionIds = new() { 10 } },
               new() { QuestionId = 2, SelectedAnswerOptionIds = new() { 20 } },
               new() { QuestionId = 3, SelectedAnswerOptionIds = new() { 31 } },
           }
       };

       // Act: Submit two correct answers and one incorrect answer.
       await controller.Submit(submission);

       // Assert: Verify that the calculated score is two out of three.
       Assert.NotNull(savedAttempt);
       Assert.Equal(2, savedAttempt.Score);
       Assert.Equal(3, savedAttempt.TotalQuestions);
    }


    // Test 3: Checks that a valid flashcard deck can be created and saved.
    [Fact]
    public async Task CreateFlashcardDeck_ValidNote_ReturnsTrue()
    {
        // Arrange: Set up the test database, repository, and flashcard deck.
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new QuizDbContext(options);

        var repository = new FlashcardDeckRepository(
            db, NullLogger<FlashcardDeckRepository>.Instance);
        
        var deck = new FlashcardDeck
        {
            Name = "Test deck",
            Description = "This is a test deck.",
            UserId = "test-user"
        };

        // Act: Create the flashcard deck.
        var result = await repository.Create(deck);

        // Assert: Verifies that the deck were created and saved
        Assert.True(result);
        Assert.Equal(1, await db.FlashcardDecks.CountAsync());
    }

    // Test 4: Checks that a flashcard can be added to an existing deck.
    [Fact]
    public async Task AddFlashcardToDeck_ValidFlashcard_SaveDeckId()
    {
        // Arrange: Set up the test database and create a flashcard deck.
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new QuizDbContext(options);

        var deck = new FlashcardDeck
        {
            Name = "Test deck",
            UserId = "test-user"
        };

        db.FlashcardDecks.Add(deck);
        await db.SaveChangesAsync();

        var flashcard = new Flashcard
        {
            QuestionText = "What is ASP.NET?",
            AnswerText = "A web application framework.",
            UserId = "test-user",
            DeckId = deck.FlashcardDeckId
        };

        // Act Add the flashcard to the deck and save it.
        db.Flashcards.Add(flashcard);
        await db.SaveChangesAsync();

        // Assert: Verify that the flashcard belongs to the correct deck.
        var savedFlashcard = await db.Flashcards
            .FirstOrDefaultAsync(card => card.FlashcardId == flashcard.FlashcardId);

        Assert.NotNull(savedFlashcard);
        Assert.Equal(deck.FlashcardDeckId, savedFlashcard.DeckId);    
    }

    // Test 5: Checks that a valid note can be created and saved.
    [Fact]
    public async Task CreateStudyNote_ValidNote_ReturnsTrue()
    {
        // Arrange: Creating testdatabase, repository and test data
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new QuizDbContext(options);

        var repository = new StudyNoteRepository(
            db, NullLogger<StudyNoteRepository>.Instance);
        
        var note = new StudyNote
        {
            Title = "Test note",
            Content = "This is a test note.",
            UserId = "test-user"
        };

        // Act: Call the Create-metoden
        var result = await repository.Create(note);

        // Assert: Verifies that the notes were created and saved
        Assert.True(result);
        Assert.Equal(1, await db.StudyNotes.CountAsync());
    }

    // Test 6: Checks that a user can retrieve their own note by ID.
    [Fact]
    public async Task GetStudyNoteById_ExistingNote_ReturnsNote()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            using var db = new QuizDbContext(options);

            var repository = new StudyNoteRepository(
                db,
                NullLogger<StudyNoteRepository>.Instance);
            

            var note = new StudyNote
            {
                Title = "Test note",
                Content = "Test content",
                UserId = "test-user"
            };

            db.StudyNotes.Add(note);
            await db.SaveChangesAsync();

            // Act
            var result = await repository.GetById(note.StudyNoteId, "test-user");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test note", result.Title);
        }
}        