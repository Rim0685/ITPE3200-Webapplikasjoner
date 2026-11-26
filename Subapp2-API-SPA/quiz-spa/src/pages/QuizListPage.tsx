import { Link } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import type { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';
import { QuizFilters } from '../components/quiz/QuizFilters';

export const QuizListPage: React.FC = () => {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const loadQuizzes = async (search?: string) => {
    try {
      setLoading(true);
      const data = await quizService.getQuizzes(search);
      setQuizzes(data);
      setError(null);
    } catch {
      setError('Could not load the quizzes.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadQuizzes();
  }, []);

  if (loading) return <div className="container" role="status">Loading quizzes...</div>;
  if (error) return <div className="container"><p role="alert">{error}</p></div>;

  return (
    <div className="container">
        <h1>Quizzes</h1>
        <p className="muted">Pick a quiz and answer the questions.</p>

      <QuizFilters onSearch={(term) => loadQuizzes(term)} />

      {quizzes.length === 0 ? (
        <p className="muted">No quizzes are available right now.</p>
      ) : (
        <div className="grid">
          {quizzes.map((quiz) => {
            const quizId = quiz.id || (quiz as any).quizId;

            return (
              <div className="card" key={quizId}>
                <h3>{quiz.title}</h3>
                {quiz.description && <p className="muted">{quiz.description}</p>}
                <p>
                  <span className="pill">Attempts: {quiz.attemptsCount || 0}</span>{' '}
                  <span className="pill">
                    Last result: {quiz.lastResult !== undefined ? `${quiz.lastResult} points` : 'No attempts'}
                  </span>
                </p>
                <Link to={`/quiz/${quizId}`} className="btn btn-dark">Start quiz</Link>
                {quiz.isSolutionAvailable && quiz.lastAttemptId != null && (
                  <Link
                    to={`/quiz/${quizId}/solution?attemptId=${quiz.lastAttemptId}`}
                    className="btn btn-light"
                  >
                    View solution
                  </Link>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};