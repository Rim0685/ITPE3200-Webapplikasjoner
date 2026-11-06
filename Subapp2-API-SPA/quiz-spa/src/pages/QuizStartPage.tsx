import React, { useEffect, useState } from 'react';
import type { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';

export const QuizStartPage: React.FC = () => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const quizId = 1; // Eksempel-ID

  useEffect(() => {
    quizService.getQuizById(quizId).then(setQuiz).catch(console.error);
  }, []);

  if (!quiz) return <div>Laster...</div>;

  return (
    <div style={{ padding: '20px' }}>
      <h1>{quiz.title}</h1>
      <p>{quiz.description}</p>
      <a href={`/quiz/${quiz.id}/take`}>
        <button>Start quizen</button>
      </a>
    </div>
  );
};