import { useState } from "react";
import { Link } from "react-router-dom";
import DeckForm from "./DeckForm";
import type { FlashcardDeck } from "../../types/flashcards";
import { deleteDeck, updateDeck } from "../../services/flashcardDeckService";

interface DeckCardProps {
    deck: FlashcardDeck;
    onDeckChanged: () => void;
}

function DeckCard({ deck, onDeckChanged }: DeckCardProps) {
    // Controls whether the edit form is visible
    const [isEditing, setIsEditing] = useState(false);

    const handleDelete = async () => {
        // Deleting a deck also deletes its flashcards, so ask first
        const confirmed = window.confirm(
            `Delete "${deck.name}"? Its ${deck.cardCount} flashcard(s) will also be deleted.`
        );

        if (!confirmed) {
            return;
        }

        try {
            await deleteDeck(deck.flashcardDeckId);
            onDeckChanged();
        } catch {
            console.error("Failed to delete deck");
        }
    };

    return (
        <div className="card">
            <span className="pill">
                {deck.cardCount} {deck.cardCount === 1 ? "card" : "cards"}
            </span>
            <h2>{deck.name}</h2>
            {deck.description && <p>{deck.description}</p>}

            <Link to={`/decks/${deck.flashcardDeckId}`} className="btn btn-dark">
                Open
            </Link>

            <button
                type="button"
                className="btn btn-light"
                onClick={() => setIsEditing(!isEditing)}
            >
                {isEditing ? "Cancel" : "Edit"}
            </button>

            <button
                type="button"
                className="btn btn-danger"
                onClick={handleDelete}
            >
                Delete
            </button>

            {isEditing && (
                <DeckForm
                    initialName={deck.name}
                    initialDescription={deck.description ?? ""}
                    submitLabel="Save changes"
                    onSubmit={async (name, description) => {
                        await updateDeck(
                            deck.flashcardDeckId,
                            name,
                            description || null
                        );
                        setIsEditing(false);
                        onDeckChanged();
                    }}
                />
            )}
        </div>
    );
}

export default DeckCard;