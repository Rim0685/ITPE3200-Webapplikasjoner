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
    <form className="quiz-filters" onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Search for a quiz..."
        value={text}
        onChange={(e) => setText(e.target.value)}
      />
      <button type="submit" className="btn btn-dark">Search</button>
    </form>
  );
};