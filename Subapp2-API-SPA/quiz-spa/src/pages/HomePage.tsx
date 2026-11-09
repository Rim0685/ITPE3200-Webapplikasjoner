import { Link } from "react-router-dom"; 

function HomePage() {
    return (
        <div className="container">
            <div className="card">
                <h1>Study Platform</h1>
                <p>Practice course content with quizzes and flashcards.</p>

                <Link to="/quizzes" className="btn tbn-dark">
                    Go to Quizzes
                </Link>

                <Link to="/flashcards" className="btn tbn-light">
                    Go to Flashcards
                </Link>
            </div>
        </div>
    );
}

export default HomePage;