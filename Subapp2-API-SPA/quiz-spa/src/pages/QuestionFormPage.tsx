import React, { useState } from 'react';

export const QuestionFormPage: React.FC = () => {
  const [questionText, setQuestionText] = useState('');

  return (
    <form className="question-form" onSubmit={(e) => e.preventDefault()}>
      <h1>Skjema for Spørsmål</h1>
      <label htmlFor="q-text">Spørsmålstekst:</label>
      <input 
        id="q-text" 
        type="text" 
        value={questionText} 
        onChange={(e) => setQuestionText(e.target.value)} 
      />
      <button type="submit">Lagre spørsmål</button>
    </form>
  );
};