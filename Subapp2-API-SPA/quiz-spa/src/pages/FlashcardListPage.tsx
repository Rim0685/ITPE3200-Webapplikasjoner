import { useEffect, useState } from "react";
import type { Flashcard } from "../types/flashcards";
import { getFlashcards } from "../services/flashcardService";
import FlashcardCard from "../components/flashcards/FlashcardCard";
import FlashcardForm from "../components/flashcards/FlashcardForm";

function FlashcardListPage() {
    const [flashcards, setFlashcards] = useState<Flashcard[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadFlashcards() {
            try {
                const data = await getFlashcards();
                setFlashcards(data);
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
            const data = await getFlashcards();
            setFlashcards(data);
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
        <div>
            <h1>My Flashcards</h1>

            {/* Form for creating a new flashcard */}
            <FlashcardForm onFlashcardCreated={refreshFlashcards}/>

            {flashcards.length === 0 ? (
                <p>You have no flashcards yet.</p>
            ) : (
                flashcards.map((flashcard) => (
                    <FlashcardCard
                        key={flashcard.flashcardId}
                        flashcard={flashcard}
                        onFlashcardUpdated={refreshFlashcards}
                    />
                ))
            )}
        </div>
    );
}

export default FlashcardListPage;