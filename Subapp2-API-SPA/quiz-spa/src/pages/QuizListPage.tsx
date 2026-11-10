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
      console.log("Hentede quizer fra API:", data);
      setQuizzes(data);
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste inn quizer.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadQuizzes();
  }, []);

  if (loading) return <div>Laster quizer...</div>;
  if (error) return <div style={{ color: 'red' }}>{error}</div>;

  return (
    <div style={{ padding: '20px' }}>
      <h1>Tilgjengelige Quizer</h1>
      <QuizFilters onSearch={(term) => loadQuizzes(term)} />

      {quizzes.length === 0 ? (
        <p>Ingen quizer funnet.</p>
      ) : (
        <ul>
          {quizzes.map((quiz) => {
            console.log("Quiz-objekt:", quiz);
            return (
              <li key={quiz.id} style={{ marginBottom: '15px', borderBottom: '1px solid #eee', paddingBottom: '10px' }}>
                <h3>{quiz.title}</h3>
                <p>{quiz.description}</p>
                <p>Forsøk: {quiz.attemptsCount || 0} | Siste resultat: {quiz.lastResult !== undefined ? `${quiz.lastResult} poeng` : 'Ingen forsøk'}</p>
               <Link to={`/quiz/${quiz.id || (quiz as any).quizId}`}>Start Quiz</Link>
                {quiz.isSolutionAvailable && (
                  <Link to={`/quiz/${quiz.id || (quiz as any).quizId}/solution`} style={{ marginLeft: '10px' }}>
                    Se fasit
                  </Link>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
};