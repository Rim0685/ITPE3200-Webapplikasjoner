import { Question } from '../types/quiz';

const API_URL = '/api/questions';

export const questionService = {
  async createQuestion(question: Omit<Question, 'id'>): Promise<Question> {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(question),
    });
    if (!response.ok) throw new Error('Kunne ikke opprette spørsmål.');
    return response.json();
  },

  async updateQuestion(id: number, question: Question): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(question),
    });
    if (!response.ok) throw new Error('Kunne ikke oppdatere spørsmålet.');
  },

  async deleteQuestion(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Kunne ikke slette spørsmålet.');
  }
};