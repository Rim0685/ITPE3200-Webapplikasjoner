import React from 'react';
import { useLocation, useParams, Link } from 'react-router-dom';

export const QuizResultPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const location = useLocation();
  
  // Henter resultatet fra navigate-state (sendt fra TakeQuizPage)
  const result = location.state?.result;

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h1>Quiz Resultater</h1>
      
      {result ? (
        <div style={{ background: '#f9f9f9', padding: '20px', borderRadius: '8px', marginTop: '20px', border: '1px solid #ddd' }}>
          <h2>Poengsum: {result.score ?? result.Score ?? 'Ikke oppgitt'}</h2>
          <p>{result.feedback ?? result.Feedback ?? 'Takk for din innsending!'}</p>

          {/* Viser detaljer per spørsmål hvis det finnes i responsen */}
          {(result.questionResults || result.QuestionResults) && (
            <div style={{ marginTop: '20px' }}>
              <h3>Detaljer per spørsmål:</h3>
              <ul style={{ paddingLeft: '20px' }}>
                {(result.questionResults || result.QuestionResults).map((qRes: any, index: number) => (
                  <li key={index} style={{ marginBottom: '8px' }}>
                    Spørsmål ID {qRes.questionId ?? qRes.QuestionId}:{' '}
                    <strong style={{ color: (qRes.isCorrect ?? qRes.IsCorrect) ? 'green' : 'red' }}>
                      {(qRes.isCorrect ?? qRes.IsCorrect) ? 'Riktig' : 'Feil'}
                    </strong>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      ) : (
        <p style={{ color: 'gray', marginTop: '20px' }}>Ingen resultatdata funnet for denne quizen.</p>
      )}

      <div style={{ marginTop: '30px' }}>
        <Link 
          to="/quizzes" 
          style={{ padding: '10px 20px', background: '#007bff', color: '#fff', textDecoration: 'none', borderRadius: '4px' }}
        >
          Tilbake til oversikten
        </Link>
      </div>
    </div>
  );
};