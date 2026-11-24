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
        <nav className="nav">
            {/* Link is used instead of <a> to navigate without reloading the SPA */}
            <Link className="logo" to="/">quizapp</Link>

            <div className="nav-links">
                <Link to="/">Home</Link>
                <Link to="/quizzes">Quizzes</Link>
                <Link to="/flashcards">Flashcards</Link>
                <Link to="/decks">Decks</Link>

                {isLoggedIn ? (
                    // Show Profile and Logout when the user has a token
                    <>
                        <Link to="/profile">Profile</Link>
                            <button
                                type="button"
                                className="btn btn-light"
                                onClick={handleLogout}
                            >
                                Log out
                            </button>
                    </>
                ) : (
                    // Show Register and Log in when the user is logged out
                    <>
                        <Link to="/register">Register</Link>
                        <Link to="/login">Log in</Link>
                    </>
                )}
            </div>
        </nav>
    );
}

export default Navbar;