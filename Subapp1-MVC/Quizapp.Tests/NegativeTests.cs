using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Quizapp.Controllers;
using Quizapp.DAL;
using Quizapp.Models;
using Xunit;

namespace Subapp1_MVC.Tests
{
    public class NegativeTests
    {
        // Test 1: Verifies that Index() returns status code 500 when the repository returns null
        [Fact]
        public async Task Index_WhenQuizzesIsNull_ReturnsStatusCode500()
        {
            // Arrange: Creates mocks for the repository and logger, and forces GetAll() to return null
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetAll().Returns(Task.FromResult<IEnumerable<Quiz>>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act: Calls Index() on the controller
            var result = await controller.Index();

            // Assert:   Confirms the result is an ObjectResult with status code 500
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // Test 2: Verifies that Details() returns NotFound with an error message when quiz ID 99 does not exist
        [Fact]
        public async Task Details_WhenQuizNotFound_ReturnsNotFound()
        {
            // Arrange:Simulates that quiz ID 99 is not found in the database (returns null)
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetQuizById(99).Returns(Task.FromResult<Quiz>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act: Requests details for quiz 99
            var result = await controller.Details(99);

            // Assert: Confirms the controller returns a NotFoundObjectResult with the expected message
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The quiz was not found.", notFoundResult.Value);
        }

        // Test 3: Verifies that Play() returns NotFound with an error message when quiz ID 99 does not exist
        [Fact]
        public async Task Play_WhenQuizNotFound_ReturnsNotFound()
        {
            // Arrange: Simulates that quiz ID 99 does not exist (returns null)
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetQuizById(99).Returns(Task.FromResult<Quiz>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act: Attempts to start quiz 99
            var result = await controller.Play(99);

            // Assert: Confirms the result is a NotFoundObjectResult with the expected message
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The quiz was not found.", notFoundResult.Value);
        }

        // Test 4: Verifies that Index() throws an exception when the database fails ("Database crash")
        [Fact]
        public async Task Index_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulates GetAll() failing with an exception
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetAll().Returns(Task.FromException<IEnumerable<Quiz>>(new Exception("Database crash")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert: Verifies that an exception is thrown when Index() is called
            await Assert.ThrowsAsync<Exception>(async () => await controller.Index());
        }

        // Test 5: Verifies that Details() throws an exception on a database timeout
        [Fact]
        public async Task Details_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulates a database timeout for any quiz ID
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetQuizById(Arg.Any<int>()).Returns(Task.FromException<Quiz>(new Exception("Database timeout")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert: Verifies that an exception is thrown when Details(1) is called
            await Assert.ThrowsAsync<Exception>(async () => await controller.Details(1));
        }

        // Test 6:Verifies that Play() throws an exception when the database connection is lost
        [Fact]
        public async Task Play_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulates a lost database connection for any quiz ID
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();

            mockRepo.GetQuizById(Arg.Any<int>()).Returns(Task.FromException<Quiz>(new Exception("Connection lost")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert: Verifies that an exception is thrown when Play(1) is called
            await Assert.ThrowsAsync<Exception>(async () => await controller.Play(1));
        }
    }
}