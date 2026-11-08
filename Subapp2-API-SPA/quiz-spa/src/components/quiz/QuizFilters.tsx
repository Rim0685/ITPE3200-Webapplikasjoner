import React, { useState } from 'react';

interface Props {
  onSearch: (searchTerm: string) => void;
}

export const QuizFilters: React.FC<Props> = ({ onSearch }) => {
  const [text, setText] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch(text);
  };

  return (
    <form onSubmit={handleSubmit} style={{ marginBottom: '20px' }}>
      <input 
        type="text" 
        placeholder="Søk etter quiz..." 
        value={text} 
        onChange={(e) => setText(e.target.value)} 
      />
      <button type="submit">Søk</button>
    </form>
  );
};