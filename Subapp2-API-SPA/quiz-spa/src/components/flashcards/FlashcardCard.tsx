import { useState } from "react";
import FlashcardEditForm from "./FlashcardEditForm";
import type { Flashcard } from "../../types/flashcards";
import { deleteFlashcard } from "../../services/flashcardService";

interface FlashcardCardProps {
    flashcard: Flashcard;
    onFlashcardUpdated: () => void;
}

function FlashcardCard({ 
    flashcard,
    onFlashcardUpdated
}:  FlashcardCardProps) {
    // Controls whether the edit form is visible
    const [isEditing, setIsEditing] = useState(false);

    // Handles deleting a flashcard
    const handleDelete = async () => {

        // Ask for confirmation before deleting the flashcard
        const confirmed = window.confirm(
            "Are you sure you want to delete this flashcard?"
        );

        if (!confirmed) {
            return;
        }

        try {
            // Delete the flashcard through the API
            await deleteFlashcard(flashcard.flashcardId);

            // Refresh the flashcard list after deletion
            onFlashcardUpdated();
        } catch {
            console.error("Failed to delete flashcard");
        }
    };

    return (
        <div className="card">
            <h2>{flashcard.front}</h2>
            <p>{flashcard.back}</p>

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

            {/* Show the edit form when the user clicks edit */}
            {isEditing && (
                <FlashcardEditForm
                    flashcardId={flashcard.flashcardId}
                    initialFront={flashcard.front}
                    initialBack={flashcard.back}
                    onFlashcardUpdated={() => {
                        // Close the form and refresh the flashcard list
                        setIsEditing(false);
                        onFlashcardUpdated();
                    }}
                />
            )}
        </div>
    );
}

export default FlashcardCard;