import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { Flashcard, FlashcardDeck } from "../types/flashcards";
import { getFlashcards } from "../services/flashcardService";
import { getDecks } from "../services/flashcardDeckService";
import FlashcardCard from "../components/flashcards/FlashcardCard";
import FlashcardForm from "../components/flashcards/FlashcardForm";

function FlashcardListPage() {
    const [flashcards, setFlashcards] = useState<Flashcard[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [decks, setDecks] = useState<FlashcardDeck[]>([]);

    useEffect(() => {
        async function loadFlashcards() {
            try {
                const [data, deckData] = await Promise.all([
                    getFlashcards(),
                    getDecks(),
                ]);
                setFlashcards(data);
                setDecks(deckData);
            } catch {
                setError("Could not load flashcards.");
            } finally {
                setLoading(false);
            }
        }

        loadFlashcards();
    }, []);

    // Fetch the updated flashcard list after creating a new flashcard
    const refreshFlashcards = async () => {
        try {
            const [data, deckData] = await Promise.all([
                getFlashcards(),
                getDecks(),
            ]);
            setFlashcards(data);
            setDecks(deckData);
        } catch {
            setError("Could not refresh flashcards.");
        }
    };

    if (loading) {
        return <p>Loading flashcards...</p>
    }

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <div className="container">
            <h1>My Flashcards</h1>
            <p><Link to="/decks" className="btn btn-light">My decks</Link></p>

            {/* Form for creating a new flashcard */}
            <FlashcardForm decks={decks} onFlashcardCreated={refreshFlashcards}/>

            {flashcards.length === 0 ? (
                <p>You have no flashcards yet.</p>
            ) : (
                <div className="grid">
                    {flashcards.map((flashcard) => (
                        <FlashcardCard
                            key={flashcard.flashcardId}
                            flashcard={flashcard}
                            decks={decks}
                            onFlashcardUpdated={refreshFlashcards}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}

export default FlashcardListPage;