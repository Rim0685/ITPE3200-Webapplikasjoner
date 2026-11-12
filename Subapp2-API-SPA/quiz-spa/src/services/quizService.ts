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

  /*async getQuizById(id: number): Promise<Quiz> {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error('Kunne ikke hente quizen.');
    return response.json();
  },*/

  async getQuizById(id: number): Promise<Quiz> {
    const token = localStorage.getItem('token'); // Bytt 'token' til den faktiske nøkkelen fra Local Storage
    console.log("Token hentet fra localStorage:", token); // <-- Sjekk hva som dukker opp i konsollen!

    const response = await fetch(`${API_URL}/${id}/take`, {
      method: 'GET',
      headers: {
        'Authorization': token ? `Bearer ${token}` : '',
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) throw new Error('Kunne ikke hente quizen.');
    
    // Fortell TypeScript at datastrukturen har riktig type
    const data: any = await response.json();

    // Legger til fullstendig URL og /images/-mappen på spørsmål- og svaralternativ-bilder automatisk
    const quizWithImages: Quiz = {
      ...data,
      questions: data.questions.map((q: any) => {
        const rawImgUrl = q.imageUrl ?? q.ImageUrl;
        let questionImageUrl = undefined;

        if (rawImgUrl) {
          if (rawImgUrl.startsWith('http')) {
            questionImageUrl = rawImgUrl;
          } else {
            const cleanPath = rawImgUrl.startsWith('/') ? rawImgUrl : `/${rawImgUrl}`;
            const pathWithImages = cleanPath.includes('/images/') ? cleanPath : `/images${cleanPath}`;
            questionImageUrl = `http://localhost:5268${pathWithImages}`;
          }
        }

        return {
          ...q,
          imageUrl: questionImageUrl,
          answerOptions: q.answerOptions.map((opt: any) => {
            const rawOptImgUrl = opt.imageUrl ?? opt.ImageUrl;
            let optionImageUrl = undefined;

            if (rawOptImgUrl) {
              if (rawOptImgUrl.startsWith('http')) {
                optionImageUrl = rawOptImgUrl;
              } else {
                const cleanOptPath = rawOptImgUrl.startsWith('/') ? rawOptImgUrl : `/${rawOptImgUrl}`;
                const pathWithImages = cleanOptPath.includes('/images/') ? cleanOptPath : `/images${cleanOptPath}`;
                optionImageUrl = `http://localhost:5268${pathWithImages}`;
              }
            }

            return {
              ...opt,
              imageUrl: optionImageUrl,
            };
          }),
        };
      }),
    };

    return quizWithImages;
  },

  async submitQuiz(quizId: number, submission: any): Promise<any> {
    const token = localStorage.getItem('token');
    
    const answersDict = submission?.answers || submission?.Answers || {};
    const formattedAnswers = Object.entries(answersDict).map(([qId, optIds]) => ({
      QuestionId: Number(qId),
      AnswerOptionIds: Array.isArray(optIds) ? optIds.map(Number) : [Number(optIds)]
    }));

    // Sender payload direkte uten 'submission'-wrapperen, i tilfelle C# [FromBody] tar imot DTO-en direkte
    const payload = {
      QuizId: Number(quizId),
      Answers: formattedAnswers
    };

    console.log("Sender flatt payload til /api/quizattempts:", JSON.stringify(payload));

    const response = await fetch(`http://localhost:5268/api/quizattempts`, {
      method: 'POST',
      headers: {
        'Authorization': token ? `Bearer ${token}` : '',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Feil fra server:", errorText);
      throw new Error('Innsending feilet.');
    }
    
    return response.json();
  },
  /*async submitQuiz(quizId: number, submission: QuizSubmission): Promise<any> {
    const response = await fetch(`${API_URL}/${quizId}/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(submission),
    });
    if (!response.ok) throw new Error('Innsending feilet.');
    return response.json();
  },*/

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