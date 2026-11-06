import type { Flashcard } from "../../types/flashcards";

interface FlashcardCardProps {
    flashcard: Flashcard;
}

function FlashcardCard({ flashcard }: FlashcardCardProps) {
    return (
        <div>
            <h2>{flashcard.front}</h2>
            <p>{flashcard.back}</p>
        </div>
    );
}

export default FlashcardCard;