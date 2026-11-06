import React from 'react';

export const QuizAdminPage: React.FC = () => {
  return (
    <div className="quiz-admin-page">
      <h1>Quiz Administrasjon</h1>
      <a href="/admin/quiz/new">Opprett ny quiz</a>
    </div>
  );
};