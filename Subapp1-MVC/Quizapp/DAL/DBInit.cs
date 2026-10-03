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
                            },
                            // Spørsmål 1: Tekstspørsmål med ett riktig svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Hvilke tre dimensjoner utgjør det klassiske \"magic triangle\" / prosjekttrianglet innen prosjektledelse?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "Mennesker, prosesser, teknologi", IsCorrect = false },
                            new AnswerOption { AnswerText = "System, krav, konsekvenser", IsCorrect = false },
                            new AnswerOption { AnswerText = "Tid, funksjonalitet, kostnad", IsCorrect = true },
                            new AnswerOption { AnswerText = "Kostnad, kvalitet, tid", IsCorrect = false }
                        }
                    },

                    // Spørsmål 2: Bildespørsmål med ett riktig svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Hvilken type diagram vises i bildet under?",
                        QuestionType = QuestionType.ImageSingle,
                        ImageUrl = "/images/diagram1.png", // Legg bildet ditt i wwwroot/images/
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "Klassediagram", IsCorrect = false },
                            new AnswerOption { AnswerText = "Aktivitetsdiagram", IsCorrect = false },
                            new AnswerOption { AnswerText = "Usecase-diagram", IsCorrect = false },
                            new AnswerOption { AnswerText = "Sekvensdiagram", IsCorrect = true }
                        }
                    },

                    // Spørsmål 3: Bildespørsmål med ett riktig svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Hvilken type diagram vises i bildet under?",
                        QuestionType = QuestionType.ImageSingle,
                        ImageUrl = "/images/diagram2.png", // Legg bildet ditt i wwwroot/images/[cite: 4]
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "Aktivitetsdiagram", IsCorrect = false },
                            new AnswerOption { AnswerText = "Usecase-diagram", IsCorrect = true },
                            new AnswerOption { AnswerText = "Sekvensdiagram", IsCorrect = false },
                            new AnswerOption { AnswerText = "Klassediagram", IsCorrect = false }
                        }
                    },

                    // Spørsmål 4: Tekstspørsmål med ett riktig svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Se på matrisen nedenfor hentet fra Det Smidige Manifest. Hvilken verdi passer sammen med \"Programvare som virker fremfor...\"?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "...omfattende dokumentasjon", IsCorrect = true },
                            new AnswerOption { AnswerText = "...kontraktsforhandlinger", IsCorrect = false },
                            new AnswerOption { AnswerText = "...prosesser og verktøy", IsCorrect = false },
                            new AnswerOption { AnswerText = "...å følge en plan", IsCorrect = false }
                        }
                    },

                    // Spørsmål 5: Flervalg med FLERE riktige svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Hvilke påstander om verktøyet Mermaid og diagrammer i kildekode er korrekte? (Flervalg - velg alle som gjelder)",
                        QuestionType = QuestionType.TextMultiple,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "Det er en stor ulempe å ha diagrammer i kildekoden", IsCorrect = false },
                            new AnswerOption { AnswerText = "En felles måte å modellere diagrammer på i kildekode", IsCorrect = true },
                            new AnswerOption { AnswerText = "Diagrammene blir en del av versjonsstyringen", IsCorrect = true },
                            new AnswerOption { AnswerText = "Du trenger ingen tekniske kunnskaper for å modellere Mermaid-diagrammer", IsCorrect = false }
                        }
                    },

                    // Spørsmål 6: Tekstspørsmål med ett riktig svar[cite: 4, 34]
                    new Question
                    {
                        QuestionText = "Hvilket av følgende krav til en billettautomat er et funksjonelt krav?",
                        QuestionType = QuestionType.TextSingle,
                        AnswerOptions = new List<AnswerOption>
                        {
                            new AnswerOption { AnswerText = "Det skal være et valg for utskrift av kvittering", IsCorrect = true },
                            new AnswerOption { AnswerText = "Teksten på skjermen skal være svart med hvit bakgrunn", IsCorrect = false }
                        }
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