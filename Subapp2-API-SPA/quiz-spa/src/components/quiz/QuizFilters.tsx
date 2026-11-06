import React, { useState } from 'react';

interface Props {
  onSearchChange: (search: string) => void;
  onFilterChange: (status: string) => void;
}

export const QuizFilters: React.FC<Props> = ({ onSearchChange, onFilterChange }) => {
  const [searchTerm, setSearchTerm] = useState('');

  const handleSearchInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
    onSearchChange(e.target.value);
  };

  return (
    <div className="quiz-filters" role="search">
      <label htmlFor="search-input">Søk i quizer: </label>
      <input
        id="search-input"
        type="text"
        value={searchTerm}
        onChange={handleSearchInput}
        placeholder="Søk tittel..."
      />
      <select onChange={(e) => onFilterChange(e.target.value)} aria-label="Filtrer fasitstatus">
        <option value="">Alle</option>
        <option value="available">Fasit tilgjengelig</option>
      </select>
    </div>
  );
};