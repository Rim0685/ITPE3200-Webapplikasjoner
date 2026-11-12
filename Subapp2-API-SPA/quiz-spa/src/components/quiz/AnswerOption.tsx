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
      style={{ cursor: 'pointer', margin: '8px 0', padding: '8px', border: isSelected ? '2px solid blue' : '1px solid #ccc', display: 'flex', alignItems: 'center', flexWrap: 'wrap' }}
    >
      <input 
        type={isMultipleChoice ? 'checkbox' : 'radio'} 
        checked={isSelected} 
        onChange={(e) => {
          e.stopPropagation();
          onSelect(option.id);
        }} 
        style={{ marginRight: '8px', cursor: 'pointer' }}
      />

      <span style={{ flexGrow: 1 }}>{option.text}</span>

      {option.imageUrl && (
        <div style={{ width: '100%', marginTop: '5px' }}>
          <img 
            src={option.imageUrl} 
            alt="Svaralternativ" 
            style={{ maxWidth: '150px', maxHeight: '100px', display: 'block', borderRadius: '4px', objectFit: 'contain' }} 
          />
        </div>
      )}
    </div>
  );
};