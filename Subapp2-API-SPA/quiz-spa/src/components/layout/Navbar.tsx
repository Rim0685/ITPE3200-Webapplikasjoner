import { Link, useNavigate, useLocation } from "react-router-dom";

function Navbar() {
    const navigate = useNavigate();
    useLocation();

    // Checks wheter a JWT token is stored
    const isLoggedIn = localStorage.getItem("token") !== null;

    // Removes the JWT token and redirects to the login page
    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/login");
    };

    return (
        <nav>
            {/* Link is used instead of <a> to navigate without reloading the SPA */}
            <Link to="/">Home</Link>
            <Link to="/quizzes">Quizzes</Link>
            <Link to="/flashcards">Flashcards</Link>

            {isLoggedIn ? (
                // Show Logout when the user has a token
                <button type="button" onClick={handleLogout}>
                    Logout
                </button>
            ) : (
                // Show Login and Register when the user is logged out
                <>
                    <Link to="/login">Login</Link>
                    <Link to="/register">Register</Link>
                </>
            )}
        </nav>
    );
}

export default Navbar;