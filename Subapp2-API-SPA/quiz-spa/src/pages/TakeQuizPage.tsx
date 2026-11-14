import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { Quiz, QuizSubmission } from '../types/quiz';
import { quizService } from '../services/quizService';
import { QuestionCard } from '../components/quiz/QuestionCard';

export const TakeQuizPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [answers, setAnswers] = useState<{ [questionId: number]: number[] }>({});
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchQuiz() {
      if (!id) return;
      try {
        setLoading(true);
        const quizIdNumber = Number(id);
        const data = await quizService.getQuizById(quizIdNumber);
        setQuiz(data);
      } catch (err) {
        setError('Kunne ikke laste inn quizen.');
      } finally {
        setLoading(false);
      }
    }
    fetchQuiz();
  }, [id]);

  const handleAnswerChange = (questionId: number, optionId: number) => {
    setAnswers((prev) => {
      // Hent eksisterende svar for dette spørsmålet
      const current = prev[questionId] || [];
      
      // Sjekk om det er flervalg basert på quizens spørsmålsliste
      const questions = quiz?.questions || (quiz as any)?.Questions || [];
      const currentQ = questions.find((q: any, idx: number) => {
        const qId = q.questionId ?? q.QuestionId ?? q.id ?? q.Id;
        return (qId !== undefined && qId !== null ? qId : idx + 1) === questionId;
      });
      
      const isMultiple = currentQ?.isMultipleChoice ?? currentQ?.IsMultipleChoice ?? false;

      if (isMultiple) {
        const updated = current.includes(optionId)
          ? current.filter((item) => item !== optionId)
          : [...current, optionId];
        return { ...prev, [questionId]: updated };
      }
      
      // For enkeltvalg (radio-knapper)
      return { ...prev, [questionId]: [optionId] };
    });
  };
  const handleSubmit = async () => {
    if (!quiz) return;
    try {
      const quizId = quiz.id ?? (quiz as any).QuizId ?? Number(id);
      const submission: QuizSubmission = { quizId, answers };
      
      const result = await quizService.submitQuiz(quizId, submission);
      navigate(`/quiz/${quizId}/result`, { state: { result } });
    } catch (err) {
      alert('Innsending feilet. Sjekk at backend kjører.');
    }
  };

  if (loading) return <div>Laster quiz...</div>;
  if (error) return <div style={{ color: 'red' }}>{error}</div>;
  if (!quiz) return <div>Fant ikke quizen.</div>;

  const rawQuestions = quiz.questions || (quiz as any).Questions || [];

  return (
    <div style={{ padding: '20px' }}>
      <h1>{quiz.title}</h1>
      <p>{quiz.description}</p>

      {rawQuestions.length === 0 ? (
        <p style={{ color: 'orange', fontWeight: 'bold' }}>
          Ingen spørsmål registrert i denne quizen ennå.
        </p>
      ) : (
        rawQuestions.map((q: any, idx: number) => {
          // Sikrer gyldig ID (faktisk ID eller 1-basert indeks unna 0)
          const rawQId = q.questionId ?? q.QuestionId ?? q.id ?? q.Id;
          const qId = (rawQId !== undefined && rawQId !== null) ? rawQId : (idx + 1);

          const qText = q.text ?? q.Text ?? '';
          const isMultiple = q.isMultipleChoice ?? q.IsMultipleChoice ?? false;
          const options = q.answerOptions ?? q.AnswerOptions ?? [];

          const rawImgUrl = q.imageUrl ?? q.ImageUrl;
          const questionImageUrl = rawImgUrl 
            ? (rawImgUrl.startsWith('http') ? rawImgUrl : `http://localhost:5268${rawImgUrl.startsWith('/') ? '' : '/'}${rawImgUrl}`)
            : undefined;

          const formattedQuestion = {
            id: qId,
            quizId: quiz.id ?? Number(id),
            text: qText,
            imageUrl: questionImageUrl,
            isMultipleChoice: isMultiple,
            answerOptions: options.map((opt: any, optIdx: number) => {
              const rawOptId = opt.id ?? opt.Id;
              const optId = (rawOptId !== undefined && rawOptId !== null) ? rawOptId : (optIdx + 1);
              const optImgUrl = opt.imageUrl ?? opt.ImageUrl;

              return {
                id: optId,
                text: opt.text ?? opt.Text ?? '',
                imageUrl: optImgUrl ? (optImgUrl.startsWith('http') ? optImgUrl : `http://localhost:5268${optImgUrl}`) : undefined,
                isCorrect: opt.isCorrect ?? opt.IsCorrect ?? false,
              };
            }),
          };

          return (
            <QuestionCard
              key={qId}
              question={formattedQuestion}
              selectedAnswers={answers[qId] || []}
              onAnswerChange={handleAnswerChange}
            />
          );
        })
      )}

      {rawQuestions.length > 0 && (
        <button onClick={handleSubmit} style={{ marginTop: '20px', padding: '10px 20px', cursor: 'pointer' }}>
          Send inn svar
        </button>
      )}
    </div>
  );
};