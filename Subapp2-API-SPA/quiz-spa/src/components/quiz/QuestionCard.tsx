import React from 'react';
import { Question } from '../../types/quiz';
import { AnswerOption } from './AnswerOption';

interface Props {
  question: Question;
  selectedAnswers: number[];
  onAnswerChange: (questionId: number, optionId: number) => void;
}

export const QuestionCard: React.FC<Props> = ({ question, selectedAnswers, onAnswerChange }) => {
  return (
    <div className="question-card" tabIndex={0}>
      <h3>{question.text}</h3>
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