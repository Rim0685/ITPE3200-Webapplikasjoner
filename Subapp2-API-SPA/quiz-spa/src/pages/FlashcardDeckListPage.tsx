import { useEffect, useState } from "react";
import type { FlashcardDeck } from "../types/flashcards";
import { createDeck, getDecks } from "../services/flashcardDeckService";
import DeckCard from "../components/flashcards/DeckCard";
import DeckForm from "../components/flashcards/DeckForm";

function FlashcardDeckListPage() {
    const [decks, setDecks] = useState<FlashcardDeck[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadInitialDecks() {
            try {
                setDecks(await getDecks());
            } catch {
                setError("Could not load decks.");
            } finally {
                setLoading(false);
            }
        }

        loadInitialDecks();
    }, []);

    // Fetches the decks again after a deck is created, edited or deleted
    const refreshDecks = async () => {
        try {
            setDecks(await getDecks());
        } catch {
            setError("Could not refresh decks.");
        }
    };

    if (loading) {
        return <p>Loading decks...</p>;
    }

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <div className="container">
            <h1>My Decks</h1>

            {/* Form for creating a new deck */}
            <DeckForm
                submitLabel="Create deck"
                resetAfterSubmit
                onSubmit={async (name, description) => {
                    await createDeck(name, description || null);
                    await refreshDecks();
                }}
            />

            {decks.length === 0 ? (
                <p>You have no decks yet.</p>
            ) : (
                <div className="grid">
                    {decks.map((deck) => (
                        <DeckCard
                            key={deck.flashcardDeckId}
                            deck={deck}
                            onDeckChanged={refreshDecks}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}

export default FlashcardDeckListPage;