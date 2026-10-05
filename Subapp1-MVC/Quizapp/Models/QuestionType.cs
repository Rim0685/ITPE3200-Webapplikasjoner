// This enum describes which question types the application supports.
// It decides whether the answer options are text or images, and whether one or several answers can be selected.

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