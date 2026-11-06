import React, { useState } from 'react';
import { quizService } from '../services/quizService';

export const QuizFormPage: React.FC = () => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await quizService.createQuiz({ title, description, isSolutionAvailable: false });
      alert('Quiz opprettet!');
    } catch (err) {
      alert('Feil ved lagring av quiz.');
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Opprett / Rediger Quiz</h1>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="quiz-title">Tittel: </label>
          <input 
            id="quiz-title" 
            type="text" 
            value={title} 
            onChange={(e) => setTitle(e.target.value)} 
          />
        </div>
        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="quiz-desc">Beskrivelse: </label>
          <input 
            id="quiz-desc" 
            type="text" 
            value={description} 
            onChange={(e) => setDescription(e.target.value)} 
          />
        </div>
        <button type="submit">Lagre</button>
      </form>
    </div>
  );
};