import React from 'react';

export const QuizAdminPage: React.FC = () => {
  return (
    <div style={{ padding: '20px' }}>
      <h1>Quiz Administrasjon</h1>
      <p>Her kan administrator administrere quizer og spørsmål.</p>
      <a href="/admin/quiz/new" className="btn btn-primary">
        Opprett ny quiz
      </a>
    </div>
  );
};