import React from 'react';
import type { AnswerOption as AnswerOptionType } from '../../types/quiz';

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
      style={{ cursor: 'pointer', margin: '8px 0', padding: '8px', border: isSelected ? '2px solid blue' : '1px solid #ccc' }}
    >
      {isMultipleChoice ? (
        <input type="checkbox" checked={isSelected} onChange={() => {}} />
      ) : (
        <input type="radio" checked={isSelected} onChange={() => {}} />
      )}

      {option.imageUrl && (
        <img 
          src={option.imageUrl} 
          alt="Svaralternativ" 
          style={{ width: '100px', display: 'block', marginTop: '5px' }} 
        />
      )}
      
      <span style={{ marginLeft: '8px' }}>{option.text}</span>
    </div>
  );
};