export interface Flashcard {
    flashcardId: number;
    front: string;
    back: string;
    deckId: number | null;
}

export interface FlashcardDeck {
    flashcardDeckId: number;
    name: string;
    description: string | null;
    createdAt: string;
    cardCount: number;
    // Only filled when a single deck is fetched
    flashcards: Flashcard[];
}