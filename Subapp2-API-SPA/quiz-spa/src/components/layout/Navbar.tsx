import { Link } from "react-router-dom";

function Navbar() {
    return (
        <nav>
            {/* Link is used instead of <a> to navigate without reloading the SPA */}
            <Link to="/">Home</Link>
            <Link to="/quizzes">Quizzes</Link>
            <Link to="/flashcards"></Link>
            <Link to="/login">Login</Link>
        </nav>
    );
}

export default Navbar;