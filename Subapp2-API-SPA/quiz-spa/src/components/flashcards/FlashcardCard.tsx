import type { Flashcards } from "../ ../types/flashcards";

interface FlashcarCardProps {
    flashcard: Flashcard;
}

function FlashcardCard({ flashcard }: FlashcardCardProps) {
    return (
        <div>
            <h2>{flashcard.question</h2>
            <p>{flashcard.answer}</p>
        </div>
    );
}

export default FlashcardCard;