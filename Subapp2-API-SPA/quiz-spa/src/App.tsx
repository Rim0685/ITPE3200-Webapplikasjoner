import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import Layout from "./components/layout/Layout";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { QuizListPage } from "./pages/QuizListPage";
import FlashcardListPage from "./pages/FlashcardListPage";


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
              localStorage.getItem("token") ? (
                <QuizListPage />  
              ) : (
                <Navigate to="/login" replace />
              )   
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