import { Link } from "react-router-dom";

function HomePage() {
    return (
        <div className="container">
            <h1>Study dashboard</h1>
            <p className="muted">Take a quiz, or practice with your own flashcards.</p>

            <div className="grid">
                <div className="card">
                    <h3>Quizzes</h3>
                    <p className="muted">Answer questions and see your score.</p>
                    <Link to="/quizzes" className="btn btn-dark">
                        View quizzes
                    </Link>
                </div>

                <div className="card">
                    <h3>Flashcards</h3>
                    <p className="muted">
                        Make your own cards and group them in decks.
                    </p>
                    <Link to="/flashcards" className="btn btn-dark">
                        My flashcards
                    </Link>
                    <Link to="/decks" className="btn btn-light">
                        My decks
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default HomePage;