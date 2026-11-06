export interface AnswerOption {
  id: number;
  text: string;
  imageUrl?: string;
  isCorrect?: boolean;
}

export interface Question {
  id: number;
  quizId: number;
  text: string;
  isMultipleChoice: boolean;
  answerOptions: AnswerOption[];
}

export interface Quiz {
  id: number;
  title: string;
  description: string;
  isSolutionAvailable: boolean;
  lastResult?: number;
  attemptsCount?: number;
  questions?: Question[];
}

export interface QuizSubmission {
  quizId: number;
  answers: { [questionId: number]: number[] };
}