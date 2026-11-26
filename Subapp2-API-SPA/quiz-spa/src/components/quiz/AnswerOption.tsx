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
    >
      <input
        type={isMultipleChoice ? 'checkbox' : 'radio'}
        checked={isSelected}
        onChange={(e) => {
          e.stopPropagation();
          onSelect(option.id);
        }}
        onClick={(e) => e.stopPropagation()}
      />

      <span>{option.text}</span>

      {option.imageUrl && (
        <div className="answer-image-wrapper">
          <img
            className="answer-image"
            src={option.imageUrl}
            alt="Answer option"
          />
        </div>
      )}
    </div>
  );
};