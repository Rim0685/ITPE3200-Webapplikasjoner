import { useEffect, useState } from 'react';
import { Link, useParams, useSearchParams } from 'react-router-dom';
import type { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';

interface QuestionSolution {
  questionId: number;
  text: string;
  explanation: string;
  selectedAnswerOptionIds: number[];
  correctAnswerOptionIds: number[];
  wasFullyCorrect: boolean;
}

interface QuizSolution {
  quizAttemptId: number;
  questions: QuestionSolution[];
}

export function QuizSolutionPage() {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();

  const quizId = Number(id);
  const attemptId = Number(searchParams.get('attemptId'));

  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [solution, setSolution] = useState<QuizSolution | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;

    async function loadSolution() {
      setLoading(true);
      setError('');

      try {
        if (
          !Number.isInteger(quizId) || quizId <= 0 ||
          !Number.isInteger(attemptId) || attemptId <= 0
        ) {
          throw new Error('Ugyldig quiz eller quizforsøk.');
        }

        const [quizData, solutionData] = await Promise.all([
          quizService.getQuizById(quizId),
          quizService.getSolution(attemptId)
        ]);

        if (!cancelled) {
          setQuiz(quizData);
          setSolution(solutionData);
        }
      } catch (err) {
        if (!cancelled) {
          setError(
            err instanceof Error
              ? err.message
              : 'Kunne ikke hente fasiten.'
          );
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadSolution();

    return () => {
      cancelled = true;
    };
  }, [quizId, attemptId]);

  if (loading) return <p>Laster fasit...</p>;

  if (error) {
    return (
      <div>
        <p role="alert">{error}</p>
        <Link to="/quizzes">Tilbake til oversikten</Link>
      </div>
    );
  }

  if (!quiz || !solution) return <p>Fant ikke fasiten.</p>;

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h1>Fasit: {quiz.title}</h1>

      {solution.questions.map((answer, index) => {
        const question = quiz.questions?.find(
          question => question.id === answer.questionId
        );

        const selectedOptions = question?.answerOptions.filter(option =>
          answer.selectedAnswerOptionIds.includes(option.id)
        ) ?? [];

        const correctOptions = question?.answerOptions.filter(option =>
          answer.correctAnswerOptionIds.includes(option.id)
        ) ?? [];

        return (
          <section
            key={answer.questionId}
            style={{
              padding: '20px',
              marginBottom: '20px',
              border: '1px solid #ddd',
              borderRadius: '8px',
              background: '#fff'
            }}
          >
            <h2>{index + 1}. {answer.text || question?.text}</h2>

            {!question ? (
              <p>Svaralternativene for dette spørsmålet kunne ikke hentes.</p>
            ) : (
              <>
                <h3>Ditt svar</h3>

                {selectedOptions.length === 0 ? (
                  <p>Ikke besvart.</p>
                ) : (
                  <ul>
                    {selectedOptions.map(option => (
                      <li key={option.id}>
                        {option.text}
                        {option.imageUrl && (
                          <img
                            src={option.imageUrl}
                            alt={option.text || 'Valgt bildealternativ'}
                            style={{ display: 'block', maxWidth: '220px' }}
                          />
                        )}
                      </li>
                    ))}
                  </ul>
                )}

                <h3>Riktige svar</h3>

                {correctOptions.length === 0 ? (
                  <p>Ingen riktige svar ble returnert for spørsmålet.</p>
                ) : (
                  <ul>
                    {correctOptions.map(option => (
                      <li key={option.id}>
                        {option.text}
                        {option.imageUrl && (
                          <img
                            src={option.imageUrl}
                            alt={option.text || 'Riktig bildealternativ'}
                            style={{ display: 'block', maxWidth: '220px' }}
                          />
                        )}
                      </li>
                    ))}
                  </ul>
                )}
              </>
            )}

            {answer.explanation && <p>{answer.explanation}</p>}
          </section>
        );
      })}

      <Link to="/quizzes">Tilbake til oversikten</Link>
    </div>
  );
}