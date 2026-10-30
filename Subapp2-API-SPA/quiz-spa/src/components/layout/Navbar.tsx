import { Link } from "react-router-dom";

function Navbar() {
    return (
        <nav>
            {/* Link is used instead of <a> to navigate without reloading the SPA */}
            <Link to="/">Home</Link>
            <Link to="/quizzes">Quizzes</Link>
            <Link to="/flashcards">Flashcards</Link>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
        </nav>
    );
}

export default Navbar;