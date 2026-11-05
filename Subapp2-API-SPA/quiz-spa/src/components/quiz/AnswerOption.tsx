import React from 'react';
import { AnswerOption as AnswerOptionType } from '../../types/quiz';

interface Props {
  option: AnswerOptionType;
  isMultipleChoice: boolean;
  isSelected: boolean;
  onSelect: (optionId: number) => void;
}

export const AnswerOption: React.FC<Props> = ({ option, isMultipleChoice, isSelected, onSelect }) => {
  return (
    <div 
      className={`answer-option ${isSelected ? 'selected' : ''}`}
      onClick={() => onSelect(option.id)}
    >
      {/* 1. Konkrete type betinget rendering: Ternær operator ? : for radioknapp vs avkrysningsboks */}
      {isMultipleChoice ? (
        <input 
          type="checkbox" 
          checked={isSelected} 
          onChange={() => onSelect(option.id)} 
          aria-label={option.text}
        />
      ) : (
        <input 
          type="radio" 
          checked={isSelected} 
          onChange={() => onSelect(option.id)} 
          aria-label={option.text}
        />
      )}

      {option.imageUrl && (
        <img 
          src={option.imageUrl} 
          alt="Svaralternativ" 
          className={`answer-image ${isSelected ? 'highlighted-image' : ''}`} 
        />
      )}
      
      <span>{option.text}</span>
    </div>
  );
};