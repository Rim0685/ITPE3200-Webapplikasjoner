import React, { useState } from 'react';

export const QuizFormPage: React.FC = () => {
  const [title, setTitle] = useState('');

  return (
    <form className="quiz-form" onSubmit={(e) => e.preventDefault()}>
      <h1>Opprett / Rediger Quiz</h1>
      <label htmlFor="quiz-title">Tittel:</label>
      <input 
        id="quiz-title" 
        type="text" 
        value={title} 
        onChange={(e) => setTitle(e.target.value)} 
      />
      <button type="submit">Lagre</button>
    </form>
  );
};