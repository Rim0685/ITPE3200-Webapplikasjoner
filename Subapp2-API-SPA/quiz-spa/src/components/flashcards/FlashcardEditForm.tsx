import { useState } from "react";
import { updateFlashcard } from "../../services/flashcardService";

interface FlashcardEditFormProps {
    flashcardId: number;
    initialFront: string;
    initialBack: string;
    onFlashcardUpdated: () => void;
}

function FlashcardEditForm({
    flashcardId,
    initialFront,
    initialBack,
    onFlashcardUpdated
}: FlashcardEditFormProps) {
    // Initialize the form with the existing flashcard values
    const [front, setFront] = useState(initialFront);
    const [back, setBack] = useState(initialBack);

    // Handles saving changes to an existing flashcard
    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
    
        try {
            // Save the updated flashcard through the API
            await updateFlashcard(flashcardId, front, back);

            onFlashcardUpdated();
        } catch {
            console.error("Failed to update flashcard");
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

            <button type="submit" className="btn btn-dark">
                Save changes
            </button>
        </form>
    );
}

export default FlashcardEditForm;