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
                Title = "Grunnleggende IT",
                Description = "En quiz med grunnleggende spørsmål om IT.",

                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Hva står HTML for?",
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
                        QuestionText = "Hvilke av disse er programmeringsspråk?",
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
                    }
                }
            };

            context.Quizzes.Add(quiz);
            context.SaveChanges();
        }
    }
}