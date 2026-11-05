import React, { useEffect, useState } from 'react';
import { Quiz } from '../types/quiz';
import { quizService } from '../services/quizService';
import { QuestionCard } from '../components/quiz/QuestionCard';

export const TakeQuizPage: React.FC = () => {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [answers, setAnswers] = useState<{ [questionId: number]: number[] }>({});
  const quizId = 1;

  useEffect(() => {
    quizService.getQuizById(quizId).then(setQuiz).catch(console.error);
  }, [quizId]);

  const handleAnswerChange = (questionId: number, optionId: number) => {
    const question = quiz?.questions?.find(q => q.id === questionId);
    if (!question) return;

    setAnswers(prev => {
      const current = prev[questionId] || [];
      if (question.isMultipleChoice) {
        const updated = current.includes(optionId) 
          ? current.filter(id => id !== optionId) 
          : [...current, optionId];
        return { ...prev, [questionId]: updated };
      } else {
        return { ...prev, [questionId]: [optionId] };
      }
    });
  };

  const handleSubmit = async () => {
    if (!quiz) return;
    try {
      await quizService.submitQuiz({ quizId: quiz.id, answers });
      window.location.href = `/quiz/${quiz.id}/result`;
    } catch (err) {
      alert('Innsending feilet.');
    }
  };

  if (!quiz) return <div>Laster quiz...</div>;

  return (
    <div className="take-quiz-page">
      <h1>{quiz.title}</h1>
      {quiz.questions?.map(question => (
        <QuestionCard 
          key={question.id} 
          question={question} 
          selectedAnswers={answers[question.id] || []} 
          onAnswerChange={handleAnswerChange} 
        />
      ))}
      <button onClick={handleSubmit}>Send inn svar (tillater ubesvarte)</button>
    </div>
  );
};