import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./components/layout/Layout";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
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
          <Route path="/flashcards" element={<FlashcardListPage />} />
        </Route>
      </Routes>
    </BrowserRouter>                          
  );
}

export default App;