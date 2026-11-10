import { useState } from "react";
import { createFlashcard } from "../../services/flashcardService";
import { createDeck } from "../../services/flashcardDeckService";
import type { FlashcardDeck } from "../../types/flashcards";

interface FlashcardFormProps {
    decks: FlashcardDeck[];
    // Preselects a deck, e.g. when the form is shown on a deck's page
    defaultDeckId?: number | null;
    onFlashcardCreated: () => void;
}

function FlashcardForm({
    decks,
    defaultDeckId = null,
    onFlashcardCreated
}: FlashcardFormProps) {
    const [front, setFront] = useState("");
    const [back, setBack] = useState("");
    const [selectedDeck, setSelectedDeck] = useState(
        defaultDeckId !== null ? String(defaultDeckId) : ""
    );
    const [newDeckName, setNewDeckName] = useState("");
    const [error, setError] = useState("");

    // Handles the form submission and creates a new flashcard
    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError("");

        try {
            let deckId: number | null =
                selectedDeck === "" ? null : Number(selectedDeck);

            // A typed deck name creates a new deck first, and the card goes into it
            const trimmedName = newDeckName.trim();

            if (trimmedName) {
                const deck = await createDeck(trimmedName, null);
                deckId = deck.flashcardDeckId;
            }

            await createFlashcard(front, back, deckId);

            setFront("");
            setBack("");
            setNewDeckName("");

            onFlashcardCreated();
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to create flashcard"
            );
        }
    };

    return (
        <form className="flashcard-form card" onSubmit={handleSubmit}>
            <div>
                <label htmlFor="front">Question</label>
                <input
                    id="front"
                    type="text"
                    value={front}
                    onChange={(event) => setFront(event.target.value)}
                    required
                />
            </div>

            <div>
                <label htmlFor="back">Answer</label>
                <input
                    id="back"
                    type="text"
                    value={back}
                    onChange={(event) => setBack(event.target.value)}
                    required
                />
            </div>

            <div>
                <label htmlFor="deck">Deck</label>
                <select
                    id="deck"
                    value={selectedDeck}
                    onChange={(event) => setSelectedDeck(event.target.value)}
                >
                    <option value="">No deck</option>
                    {decks.map((deck) => (
                        <option
                            key={deck.flashcardDeckId}
                            value={deck.flashcardDeckId}
                        >
                            {deck.name}
                        </option>
                    ))}
                </select>
            </div>

            <div>
                <label htmlFor="newDeckName">Or create a new deck</label>
                <input
                    id="newDeckName"
                    type="text"
                    maxLength={100}
                    placeholder="New deck name (overrides the choice above)"
                    value={newDeckName}
                    onChange={(event) => setNewDeckName(event.target.value)}
                />
            </div>

            {error && <p role="alert">{error}</p>}

            <button type="submit" className="btn btn-dark">
                Create flashcard
            </button>
        </form>
    );
}

export default FlashcardForm;