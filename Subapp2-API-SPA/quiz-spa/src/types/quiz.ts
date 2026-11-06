export interface AnswerOption {
  id: number;
  text: string;
  imageUrl?: string;
  isCorrect?: boolean; // Brukes i fasit/admin
}

export interface Question {
  id: number;
  quizId: number;
  text: string;
  isMultipleChoice: boolean; // Avgjør radioknapp vs avkrysningsboks
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
  answers: { [questionId: number]: number[] }; // questionId -> array av valgte answerOption-ID-er
}

export interface QuestionResultEvaluation {
  questionId: number;
  questionText: string;
  userAnswers: number[];
  correctAnswers: number[];
  isCorrect: boolean;
  points: number;
}

export interface QuizResultResponse {
  quizId: number;
  totalScore: number;
  maxScore: number;
  evaluations: QuestionResultEvaluation[];
}