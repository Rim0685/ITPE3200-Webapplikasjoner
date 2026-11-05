import React, { useEffect, useState } from 'react';
import { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';
import { QuizFilters } from '../components/quiz/QuizFilters';

export const QuizListPage: React.FC = () => {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState<string>('');
  const [filter, setFilter] = useState<string>('');

  useEffect(() => {
    setLoading(true);
    quizService.getAllQuizzes(search, filter)
      .then(data => { setQuizzes(data); setLoading(false); })
      .catch(err => { setError(err.message); setLoading(false); });
  }, [search, filter]);

  if (loading) return <div role="status">Laster quizer...</div>;
  if (error) return <div role="alert" className="error">API-feil: {error}</div>;

  return (
    <div className="quiz-list-page">
      <h1>Tilgjengelige Quizer</h1>
      <QuizFilters onSearchChange={setSearch} onFilterChange={setFilter} />

      {quizzes.length === 0 ? (
        <p>Ingen quizer funnet.</p>
      ) : (
        <ul>
          {quizzes.map(quiz => (
            <li key={quiz.id}>
              <h2>{quiz.title}</h2>
              <p>{quiz.description}</p>
              <p>Siste resultat: {quiz.lastResult ?? 'Ingen'} | Forsøk: {quiz.attemptsCount ?? 0}</p>
              <p>Fasitstatus: {quiz.isSolutionAvailable ? 'Tilgjengelig' : 'Ikke tilgjengelig'}</p>
              <a href={`/quiz/${quiz.id}/start`}>Start Quiz</a>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};