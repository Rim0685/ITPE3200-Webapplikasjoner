// Dette er en enum som beskriver hvilke typer spørsmål applikasjonen støtter.
// Den bestemmer om svaralternativene er tekst eller bilder, og om ett eller flere svar kan velges.

namespace Quizapp.Models
{
    public enum QuestionType
    {
        TextSingle,
        TextMultiple,
        ImageSingle,
        ImageMultiple
    }
}