using Microsoft.EntityFrameworkCore;
using Quizapp.Models;

namespace Quizapp.DAL
{
    // Legger inn startdata i databasen når applikasjonen kjøres.
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var context = serviceScope.ServiceProvider
                .GetRequiredService<QuizDbContext>();

            // Oppdaterer databasen med eventuelle nye migrasjoner.
            context.Database.Migrate();

            // Hindrer at samme quiz blir lagt inn flere ganger.
            if (context.Quizzes.Any())
            {
                return;
            }

           var quiz = new Quiz
            {
                Title = "Basic IT",
                Description = "A quiz with basic IT questions.",

                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What does HTML stand for?",
                        QuestionType = QuestionType.TextSingle,

                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                AnswerText = "HyperText Markup Language",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                AnswerText = "High Transfer Machine Language",
                                IsCorrect = false
                            },
                            new AnswerOption
                            {
                                AnswerText = "Home Tool Markup Language",
                                IsCorrect = false
                            }
                        }
                    },

                    new Question
                    {
                        QuestionText = "Which of these are programming languages?",
                        QuestionType = QuestionType.TextMultiple,

                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption
                            {
                                AnswerText = "Java",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                AnswerText = "C#",
                                IsCorrect = true
                            },
                            new AnswerOption
                            {
                                AnswerText = "Microsoft Word",
                                IsCorrect = false
                            }
                        }
                    },

                    // Question 1
                    new Question
                    {
                        QuestionText = "Which three dimensions form the classic \"magic triangle\" / project triangle in project management?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "People, processes, technology", 
                                IsCorrect = false 
                                },
                            new() 
                            { 
                                AnswerText = "System, requirements, consequences", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Time, functionality, cost", 
                                IsCorrect = true },
                            new AnswerOption 
                            { 
                                AnswerText = "Cost, quality, time", 
                                IsCorrect = false }
                        }
                    },

                    // Question 2: Image question with one correct answer
                    new Question
                    {
                        QuestionText = "Which type of diagram is shown in the image below?",
                        QuestionType = QuestionType.ImageSingle,
                        ImageUrl = "/images/diagram1.png",
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "Class diagram", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Activity diagram", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Use case diagram", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Sequence diagram", 
                                IsCorrect = true 
                                }
                        }
                    },

                    // Question 3: Image question with one correct answer
                    new Question
                    {
                        QuestionText = "Which type of diagram is shown in the image below?",
                        QuestionType = QuestionType.ImageSingle,
                        ImageUrl = "/images/diagram2.png",
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "Activity diagram", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Use case diagram", 
                                IsCorrect = true 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Sequence diagram", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Class diagram", 
                                IsCorrect = false 
                                }
                        }
                    },

                    // Question 4: Text question with one correct answer
                    new Question
                    {
                        QuestionText = "Look at the matrix below taken from the Agile Manifesto. Which value pairs with \"Working software over...\"?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "...comprehensive documentation", 
                                IsCorrect = true 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "...contract negotiation", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "...processes and tools", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "...following a plan", 
                                IsCorrect = false 
                                }
                        }
                    },

                    // Question 5: Multiple choice with MULTIPLE correct answers
                    new Question
                    {
                        QuestionText = "Which statements about the tool Mermaid and diagrams as code are correct? (Multiple choice - select all that apply)",
                        QuestionType = QuestionType.TextMultiple,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "It is a major disadvantage to have diagrams in the source code", 
                                IsCorrect = false 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "A common way to model diagrams in source code", 
                                IsCorrect = true 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "Diagrams become part of version control", 
                                IsCorrect = true 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "You need no technical skills to model Mermaid diagrams", 
                                IsCorrect = false 
                                }
                        }
                    },

                    // Question 6: Text question with one correct answer
                    new Question
                    {
                        QuestionText = "Which of the following requirements for a ticket vending machine is a functional requirement?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption 
                            { 
                                AnswerText = "There shall be an option to print a receipt", 
                                IsCorrect = true 
                                },
                            new AnswerOption 
                            { 
                                AnswerText = "The text on the screen shall be black with a white background", 
                                IsCorrect = false 
                                }
                        }
                    }
                }
            };

            context.Quizzes.Add(quiz);
            context.SaveChanges();
        }
    }
}