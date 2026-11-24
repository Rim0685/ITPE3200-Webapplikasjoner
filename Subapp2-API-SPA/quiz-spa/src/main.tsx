import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './styles/site.css'
import './styles/quiz.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
