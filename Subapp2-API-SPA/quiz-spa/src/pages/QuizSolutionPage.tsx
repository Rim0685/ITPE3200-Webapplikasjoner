import React, { useEffect, useState } from 'react';
import type { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';

export const QuizSolutionPage: React.FC = () => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const quizId = 1;

  useEffect(() => {
    quizService.getQuizById(quizId).then(setQuiz).catch(console.error);
  }, []);

  if (!quiz) return <div>Laster...</div>;

  return (
    <div style={{ padding: '20px' }}>
      <h1>Fasit for {quiz.title}</h1>

      {/* Logisk AND for å vise fasit når den er tilgjengelig */}
      {quiz.isSolutionAvailable && (
        <div style={{ marginTop: '20px', padding: '15px', background: '#f0f0f0' }}>
          <h2>Rette svar og forklaringer</h2>
          <p>Fasiten er nå åpen og tilgjengelig for denne quizen.</p>
        </div>
      )}
    </div>
  );
};