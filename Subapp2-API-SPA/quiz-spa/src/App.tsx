import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import Layout from "./components/layout/Layout";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { QuizListPage } from "./pages/QuizListPage";
import FlashcardListPage from "./pages/FlashcardListPage";
import { TakeQuizPage } from "./pages/TakeQuizPage";

// Checks if the user is logged in before showing a protected page
function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem("token");

  return token ? children : <Navigate to="/login" replace />;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Pages nested inside this route share the same layout */}
        <Route element={<Layout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/quizzes"
            element={
              <ProtectedRoute>
                <QuizListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/quiz/:id"
            element={
              <ProtectedRoute>
                <TakeQuizPage />
              </ProtectedRoute>
            }
          />

          {/* Redirect users without a token to the login page */}
          <Route
            path="/flashcards"
            element={
              localStorage.getItem("token") ? (
                <FlashcardListPage />  
              ) : (
                <Navigate to="/login" replace />
              )   
            }
          />
        </Route>
      </Routes>
    </BrowserRouter>                          
  );
}

export default App;