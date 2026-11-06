import React, { useEffect, useState } from 'react';
import type { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';

export const QuizSolutionPage: React.FC = () => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const quizId = 1;

  useEffect(() => {
    quizService.getQuizById(quizId).then(setQuiz).catch(console.error);
  }, [quizId]);

  if (!quiz) return <div>Laster...</div>;

  return (
    <div className="quiz-solution-page">
      <h1>Fasit for {quiz.title}</h1>

      {/* 2. Konkrete type betinget rendering: Logisk AND && for å vise fasit når tilgjengelig */}
      {quiz.isSolutionAvailable && (
        <div className="solution-container" tabIndex={0}>
          <h2>Rette svar og forklaringer</h2>
          <p>Fasiten er nå åpen og tilgjengelig.</p>
        </div>
      )}
    </div>
  );
};