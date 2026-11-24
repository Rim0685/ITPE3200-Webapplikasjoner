import React from 'react';
import type { Question } from '../../types/quiz';
import { AnswerOption } from './AnswerOption';

interface Props {
  question: Question;
  selectedAnswers: number[];
  onAnswerChange: (questionId: number, optionId: number) => void;
}

export const QuestionCard: React.FC<Props> = ({ question, selectedAnswers, onAnswerChange }) => {
  const q = question as any; // <-- Lar deg bruke imageUrl uten at TS klager

  return (
    <div className="question-card">
      <h3>{question.text}</h3>

      {q.imageUrl && (
        <img
          className="question-image"
          src={q.imageUrl}
          alt="Question"
        />
      )}

      <div className="options-container">
        {question.answerOptions.map((option) => (
          <AnswerOption
            key={option.id}
            option={option}
            isMultipleChoice={question.isMultipleChoice}
            isSelected={selectedAnswers.includes(option.id)}
            onSelect={(optId) => onAnswerChange(question.id, optId)}
          />
        ))}
      </div>
    </div>
  );
};