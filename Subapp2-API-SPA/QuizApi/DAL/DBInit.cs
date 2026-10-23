// The DAL folder contains database configuration and data access code.
// This class adds example quiz data from Subapp 1.

using Microsoft.EntityFrameworkCore;
using QuizApi.Models;

namespace QuizApi.DAL
{
    public static class DBInit
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var context = serviceScope.ServiceProvider
                .GetRequiredService<QuizDbContext>();

            var logger = serviceScope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("DBInit");

            try
            {
                // Skips seeding when the database already contains a quiz.
                if (await context.Quizzes.AnyAsync())
                {
                    return;
                }

                var questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What does HTML stand for?",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "HyperText Markup Language",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "High Transfer Machine Language",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Home Tool Markup Language",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which of these are programming languages?",
                        AllowMultipleAnswers = true,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "Java",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "C#",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "Microsoft Word",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which three dimensions form the classic \"magic triangle\" / project triangle in project management?",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "People, processes, technology",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "System, requirements, consequences",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Time, functionality, cost",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "Cost, quality, time",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which type of diagram is shown in the image below?",
                        ImageUrl = "/images/diagram1.png",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "Class diagram",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Activity diagram",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Use case diagram",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Sequence diagram",
                                IsCorrect = true
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which type of diagram is shown in the image below?",
                        ImageUrl = "/images/diagram2.png",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "Activity diagram",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Use case diagram",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "Sequence diagram",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "Class diagram",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which value pairs with \"Working software over...\" in the Agile Manifesto?",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "...comprehensive documentation",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "...contract negotiation",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "...processes and tools",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "...following a plan",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which statements about the tool Mermaid and diagrams as code are correct? (Multiple choice - select all that apply)",
                        AllowMultipleAnswers = true,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "It is a major disadvantage to have diagrams in the source code",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                Text = "A common way to model diagrams in source code",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "Diagrams become part of version control",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "You need no technical skills to model Mermaid diagrams",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        Text = "Which of the following requirements for a ticket vending machine is a functional requirement?",
                        AllowMultipleAnswers = false,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                Text = "There shall be an option to print a receipt",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                Text = "The text on the screen shall be black with a white background",
                                IsCorrect = false
                            }
                        }
                    }
                };

                var quiz = new Quiz
                {
                    Title = "Basic IT",
                    Topic = "IT",
                    Description = "A quiz with basic IT questions."
                };

                // Connects the questions to the quiz in their original order.
                for (int i = 0; i < questions.Count; i++)
                {
                    quiz.QuizQuestions.Add(new QuizQuestion
                    {
                        Question = questions[i],
                        Order = i + 1
                    });
                }

                // Saves the quiz, question links, questions and answer options.
                context.Quizzes.Add(quiz);
                await context.SaveChangesAsync();

                logger.LogInformation(
                    "Seeded quiz {Title} with {QuestionCount} questions.",
                    quiz.Title,
                    questions.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed the quiz database.");
                throw;
            }
        }
    }
}