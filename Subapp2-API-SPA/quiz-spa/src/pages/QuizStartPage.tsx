import React, { useEffect, useState } from 'react';
import { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';

export const QuizStartPage: React.FC = () => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);

  // Eksempel på henting basert på URL-path
  const quizId = 1; 

  useEffect(() => {
    quizService.getQuizById(quizId).then(setQuiz).catch(console.error);
  }, [quizId]);

  if (!quiz) return <div>Laster startside...</div>;

  return (
    <div className="quiz-start-page">
      <h1>{quiz.title}</h1>
      <p>{quiz.description}</p>
      <a href={`/quiz/${quiz.id}/take`} className="start-btn">Start gjennomføring</a>
    </div>
  );
};