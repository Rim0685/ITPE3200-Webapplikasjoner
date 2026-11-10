import { useState } from "react";
import { updateFlashcard } from "../../services/flashcardService";
import type { FlashcardDeck } from "../../types/flashcards";

interface FlashcardEditFormProps {
    flashcardId: number;
    initialFront: string;
    initialBack: string;
    initialDeckId: number | null;
    decks: FlashcardDeck[];
    onFlashcardUpdated: () => void;
}

function FlashcardEditForm({
    flashcardId,
    initialFront,
    initialBack,
    initialDeckId,
    decks,
    onFlashcardUpdated
}: FlashcardEditFormProps) {
    // Initialize the form with the existing flashcard values
    const [front, setFront] = useState(initialFront);
    const [back, setBack] = useState(initialBack);
    const [selectedDeck, setSelectedDeck] = useState(
        initialDeckId !== null ? String(initialDeckId) : ""
    );
    const [error, setError] = useState("");

    // Handles saving changes to an existing flashcard
    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError("");

        try {
            // Save the updated flashcard through the API
            await updateFlashcard(
                flashcardId,
                front,
                back,
                selectedDeck === "" ? null : Number(selectedDeck)
            );

            onFlashcardUpdated();
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to update flashcard"
            );
        }
    };

    return (
        <form className="flashcard-form" onSubmit={handleSubmit}>
            <div>
                <label htmlFor={`front-${flashcardId}`}>Question</label>
                <input
                    id={`front-${flashcardId}`}
                    type="text"
                    value={front}
                    onChange={(event) => setFront(event.target.value)}
                    required
                />
            </div>

            <div>
                <label htmlFor={`back-${flashcardId}`}>Answer</label>
                <input
                    id={`back-${flashcardId}`}
                    type="text"
                    value={back}
                    onChange={(event) => setBack(event.target.value)}
                    required
                />
            </div>

            <div>
                <label htmlFor={`deck-${flashcardId}`}>Deck</label>
                <select
                    id={`deck-${flashcardId}`}
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

            {error && <p role="alert">{error}</p>}

            <button type="submit" className="btn btn-dark">
                Save changes
            </button>
        </form>
    );
}

export default FlashcardEditForm;