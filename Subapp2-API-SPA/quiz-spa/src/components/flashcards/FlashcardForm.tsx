import { useState } from "react";
import { createFlashcard } from "../../services/flashcardService";

interface FlashcardFormProps {
    onFlashcardCreated: () => void;
}

function FlashcardForm({ onFlashcardCreated }: FlashcardFormProps) {
    const [front, setFront] = useState("");
    const [back, setBack] = useState("");

    // Handles the form submission and creates a new flashcard
    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();

        try {
            await createFlashcard(front, back);

            setFront("");
            setBack("");

            onFlashcardCreated();
        } catch {
            console.error("Failed to create flashcard");
        }
    };    

    return (
        <form onSubmit={handleSubmit}>
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

            <button type="submit">Create flashcard</button>
        </form>
    );
}

export default FlashcardForm;