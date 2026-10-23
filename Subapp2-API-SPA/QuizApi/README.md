# QuizApi – Database and Data Access Layer

Backend for Subapp 2, built with ASP.NET Core 10, Entity Framework Core
and SQLite.

This document describes the database setup and repository interfaces.
API endpoints, authentication and the React frontend are developed
by the other group members.

## Requirements

- .NET 10 SDK.
- Run the following commands from the QuizApi directory.

## Setup on a New Machine

Restore the project packages:

```powershell
dotnet restore
```

Restore the local Entity Framework tool:

```powershell
dotnet tool restore
```

Create or update the database using the existing migrations:

```powershell
dotnet ef database update
```

Build the project:

```powershell
dotnet build
```

Start the backend in the Development environment:

```powershell
dotnet run --environment Development
```

The terminal displays the application's address.
Press Ctrl + C to stop the application.

## Database Configuration

The connection string is defined in appsettings.json under:

```text
ConnectionStrings:QuizDbContext
```

The SQLite database file is named QuizApiDatabase.db.

Run the commands from the QuizApi directory to keep the relative
database path consistent.

Subapp 1 uses its own database configuration.

## Example Data

DBInit.SeedAsync runs when the application starts in the Development
environment.

It creates the Basic IT quiz with questions, answer options and
QuizQuestion links.

Seeding is skipped if the database already contains a quiz.
Changing the seed code therefore does not update existing data.

The example quiz currently contains eight questions.
Two additional questions will be added later.

## Domain Models

| Model | Purpose |
|---|---|
| Quiz | Stores the quiz title, topic and description. |
| Question | Stores question text, an explanation and an optional image path. |
| AnswerOption | Stores an answer option and whether it is correct. |
| QuizQuestion | Links quizzes and questions and stores question order. |
| Flashcard | Stores front text, back text and the owner's user ID. |
| QuizAttempt | Stores a submitted attempt, user ID, completion time and score. |
| AttemptAnswer | Stores a question response, selected options and awarded points. |

Quiz and Question have a many-to-many relationship through QuizQuestion.
A question can be used in multiple quizzes.

AttemptAnswer and AnswerOption are linked through the
AttemptAnswerOptions join table.

## Repositories

### IQuizRepository / QuizRepository

- GetAllAsync()
- GetByIdAsync(int quizId)
- CreateAsync(Quiz quiz)
- UpdateAsync(Quiz quiz)
- DeleteAsync(int quizId)
- AddQuestionAsync(int quizId, int questionId, int order)
- UpdateQuestionOrderAsync(int quizId, int questionId, int order)
- RemoveQuestionAsync(int quizId, int questionId)

### IQuestionRepository / QuestionRepository

- GetAllAsync()
- GetByIdAsync(int questionId)
- CreateAsync(Question question)
- UpdateAsync(Question question)
- DeleteAsync(int questionId)

### IFlashcardRepository / FlashcardRepository

- GetAllAsync(string userId)
- GetByIdAsync(int flashcardId, string userId)
- CreateAsync(Flashcard flashcard)
- UpdateAsync(Flashcard flashcard, string userId)
- DeleteAsync(int flashcardId, string userId)

### IQuizAttemptRepository / QuizAttemptRepository

- CreateAsync(QuizAttempt attempt)
- GetByIdAsync(int quizAttemptId, string userId)
- GetLatestAsync(int quizId, string userId)
- CountAsync(int quizId, string userId)

Database queries and saves use async and await.

## Repository Behaviour

### Quizzes and Question Links

QuizRepository.UpdateAsync updates only the quiz title, topic
and description.

Question links are managed through separate methods.

AddQuestionAsync links an existing question to an existing quiz.
The question must not already be linked to that quiz.

UpdateQuestionOrderAsync changes the Order value of one link.
It does not automatically reorder the other questions.

RemoveQuestionAsync removes the link without deleting the question.

When creating a quiz with links to existing questions, use their
QuestionId values and leave the Question navigation properties unset.
This avoids treating existing questions as new entities.

### Questions and Answer Options

QuestionRepository.UpdateAsync expects the complete list of answer
options.

- Existing options must retain their AnswerOptionId values.
- New options must have AnswerOptionId set to 0.
- Existing options omitted from the submitted list are deleted.
- Duplicate existing option IDs and IDs belonging to other questions
  are rejected.

Updates are rejected if the question is referenced by a recorded
AttemptAnswer. This protects existing attempt history during the
normal update flow.

This check alone does not fully handle concurrent edits and submissions.

### Flashcards and Ownership

Flashcard read, update and delete operations filter by user ID.

Updates change only Front and Back. The stored owner is preserved.

The API must obtain the user ID from the authenticated user.
Repository filtering does not replace API authentication or authorisation.

### Quiz Attempts

QuizAttemptRepository stores scores supplied by the evaluation service.
It does not calculate scores or determine solution access.

Only validated, submitted attempts should be passed to CreateAsync.

SelectedOptions must reference existing answer options using their
AnswerOptionId values. The repository loads those options from the
database before saving the links.

After a successful save, the generated QuizAttemptId is copied to the
attempt object supplied by the caller.

GetLatestAsync retrieves the latest attempt by CompletedAt, using
QuizAttemptId to resolve equal timestamps.

CountAsync counts saved attempts for a particular quiz and user.

## Data Integrity

- QuizQuestion uses QuizId and QuestionId as a composite primary key.
- A quiz with recorded attempts cannot be deleted.
- A question referenced by recorded answers cannot be deleted.
- Each attempt can contain only one AttemptAnswer per question.

## Error Handling and Logging

Repositories use ILogger to record errors and relevant rejected operations.

Write methods return bool. A false result can indicate missing data,
a rejected operation or a database update failure. It does not identify
the exact cause by itself.

Read methods log exceptions and rethrow them.
The API layer must handle these failures and return suitable responses.

## Responsibilities Outside the DAL

The API and service layers are responsible for:

- Mapping models to DTOs.
- Preventing correct answers from being exposed while taking a quiz.
- Validating question content and submitted answers.
- Checking that submitted questions belong to the quiz and selected
  options belong to the correct questions.
- Calculating scores.
- Deciding when an attempt is ready to be saved.
- Enforcing solution access rules.
- Enforcing the limit of 20 flashcards per user.
- Authentication and authorisation.

## Current Status

The backend has been built and started locally.
Repository behaviour has not yet been functionally verified.

Remaining work:

- Functionally test the repositories and relationships.
- Add two questions to the example quiz.
- Complete integration with authentication and any required Identity
  relationships.
- Test the integration between the DAL, API and frontend.
- Update these instructions when the remaining backend setup is complete.