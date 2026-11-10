import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type { FlashcardDeck } from "../types/flashcards";
import { getDeck, getDecks } from "../services/flashcardDeckService";
import FlashcardCard from "../components/flashcards/FlashcardCard";
import FlashcardForm from "../components/flashcards/FlashcardForm";

function FlashcardDeckDetailPage() {
    // The deck's ID comes from the URL, e.g. /decks/3
    const { deckId } = useParams();
    const id = Number(deckId);

    const [deck, setDeck] = useState<FlashcardDeck | null>(null);
    const [decks, setDecks] = useState<FlashcardDeck[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadInitialDeck() {
            try {
                const [deckData, allDecks] = await Promise.all([
                    getDeck(id),
                    getDecks(),
                ]);

                setDeck(deckData);
                setDecks(allDecks);
            } catch {
                setError("Could not load the deck.");
            } finally {
                setLoading(false);
            }
        }

        loadInitialDeck();
    }, [id]);

    // Fetches the deck and all decks again after a card is added or changed
    const refreshDeck = async () => {
        try {
            const [deckData, allDecks] = await Promise.all([
                getDeck(id),
                getDecks(),
            ]);

            setDeck(deckData);
            setDecks(allDecks);
        } catch {
            setError("Could not refresh the deck.");
        }
    };

    if (loading) {
        return <p>Loading deck...</p>;
    }

    if (error || !deck) {
        return <p>{error || "The deck was not found."}</p>;
    }

    return (
        <div className="container">
            <p><Link to="/decks">← Back to decks</Link></p>

            <h1>{deck.name}</h1>
            {deck.description && <p>{deck.description}</p>}

            {/* New cards created here are placed in this deck */}
            <FlashcardForm
                decks={decks}
                defaultDeckId={deck.flashcardDeckId}
                onFlashcardCreated={refreshDeck}
            />

            {deck.flashcards.length === 0 ? (
                <p>This deck has no flashcards yet.</p>
            ) : (
                <div className="grid">
                    {deck.flashcards.map((flashcard) => (
                        <FlashcardCard
                            key={flashcard.flashcardId}
                            flashcard={flashcard}
                            decks={decks}
                            onFlashcardUpdated={refreshDeck}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}

export default FlashcardDeckDetailPage;