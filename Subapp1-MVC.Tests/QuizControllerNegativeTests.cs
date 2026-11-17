using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Quizapp.Controllers;
using Quizapp.Models;
using Quizapp.DAL;
using Xunit;

namespace Subapp1_MVC.Tests
{
    public class QuizControllerNegativeTests
    {
        // Test 1
        [Fact]
        public async Task Index_WhenQuizzesIsNull_ReturnsStatusCode500()
        {
            // Arrange
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetAll().Returns(Task.FromResult<IEnumerable<Quiz>>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act
            var result = await controller.Index();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        //Test 2 
        [Fact]
        public async Task Details_WhenQuizNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetQuizById(99).Returns(Task.FromResult<Quiz>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act
            var result = await controller.Details(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The quiz was not found.", notFoundResult.Value);
        }

        //Test 3 
        [Fact]
        public async Task Play_WhenQuizNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetQuizById(99).Returns(Task.FromResult<Quiz>(null!));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act
            var result = await controller.Play(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The quiz was not found.", notFoundResult.Value);
        }

        // Test 4  
        [Fact]
        public async Task Index_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulerer at databasen/repository kaster et unntak
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetAll().Returns(Task.FromException<IEnumerable<Quiz>>(new Exception("Database crash")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert: Sjekk at unntaket bobler opp eller håndteres
            await Assert.ThrowsAsync<Exception>(async () => await controller.Index());
        }

        // Test 5
        [Fact]
        public async Task Details_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulerer databasefeil ved henting av spesifikk quiz
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetQuizById(Arg.Any<int>()).Returns(Task.FromException<Quiz>(new Exception("Database timeout")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await controller.Details(1));
        }

        // Test 6 
        [Fact]
        public async Task Play_WhenDatabaseThrowsException_HandlesOrThrows()
        {
            // Arrange: Simulerer databasefeil når en bruker prøver å starte en quiz
            var mockRepo = Substitute.For<IQuizRepository>();
            var mockLogger = Substitute.For<ILogger<QuizController>>();
            
            mockRepo.GetQuizById(Arg.Any<int>()).Returns(Task.FromException<Quiz>(new Exception("Connection lost")));

            var controller = new QuizController(mockRepo, mockLogger);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await controller.Play(1));
        }
    }
}