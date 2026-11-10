import type { Quiz, QuizSubmission } from '../types/quiz';

//const API_URL = '/api/quizzes';
const API_URL = 'http://localhost:5268/api/quizzes';

export const quizService = {
  async getQuizzes(search?: string): Promise<Quiz[]> {
    const url = search ? `${API_URL}?search=${encodeURIComponent(search)}` : API_URL;
    const response = await fetch(url);
    if (!response.ok) throw new Error('Kunne ikke hente quizer.');
    return response.json();
  },

  async getQuizById(id: number): Promise<Quiz> {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error('Kunne ikke hente quizen.');
    return response.json();
  },
  async submitQuiz(quizId: number, submission: QuizSubmission): Promise<any> {
    const response = await fetch(`${API_URL}/${quizId}/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(submission),
    });
    if (!response.ok) throw new Error('Innsending feilet.');
    return response.json();
  },

  /*async submitQuiz(submission: QuizSubmission): Promise<any> {
    const response = await fetch(`${API_URL}/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(submission),
    });
    if (!response.ok) throw new Error('Innsending feilet.');
    return response.json();
  },*/

  async createQuiz(quiz: Omit<Quiz, 'id'>): Promise<Quiz> {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(quiz),
    });
    if (!response.ok) throw new Error('Kunne ikke opprette quiz.');
    return response.json();
  },

  async deleteQuiz(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Kunne ikke slette quizen.');
  }
};