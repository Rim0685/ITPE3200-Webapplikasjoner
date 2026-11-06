import type { Quiz, QuizResultResponse, QuizSubmission } from '../types/quiz';

const API_URL = '/api/quizzes'; // Tilpasset ASP.NET Web API endpoint

export const quizService = {
  async getAllQuizzes(searchQuery?: string, filterStatus?: string): Promise<Quiz[]> {
    const params = new URLSearchParams();
    if (searchQuery) params.append('search', searchQuery);
    if (filterStatus) params.append('status', filterStatus);

    const response = await fetch(`${API_URL}?${params.toString()}`);
    if (!response.ok) throw new Error('Kunne ikke hente quizer fra API-et.');
    return response.json();
  },

  async getQuizById(id: number): Promise<Quiz> {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error('Kunne ikke hente quizen.');
    return response.json();
  },

  async submitQuiz(submission: QuizSubmission): Promise<QuizResultResponse> {
    const response = await fetch(`${API_URL}/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(submission),
    });
    if (!response.ok) throw new Error('Feil ved innsending av quiz.');
    return response.json();
  },

  async createQuiz(quiz: Omit<Quiz, 'id'>): Promise<Quiz> {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(quiz),
    });
    if (!response.ok) throw new Error('Kunne ikke opprette quiz.');
    return response.json();
  },

  async updateQuiz(id: number, quiz: Quiz): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(quiz),
    });
    if (!response.ok) throw new Error('Kunne ikke oppdatere quizen.');
  },

  async deleteQuiz(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Kunne ikke slette quizen.');
  }
};