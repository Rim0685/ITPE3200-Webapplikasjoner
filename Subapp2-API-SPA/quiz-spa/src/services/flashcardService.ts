import { apiRequest } from "./apiClient";
import type { Flashcard } from "../types/flashcards";

export async function getFlashcards(): Promise<Flashcard[]> {
    const response = await apiRequest("/api/flashcards");

    if (!response.ok) {
        throw new Error("Failed to fetch flashcards");
    }

    return response.json();
}

export async function createFlashcard(
    front: string,
    back: string
): Promise<Flashcard> {
    const response = await apiRequest("/api/flashcards", {
        method: "POST",
        body: JSON.stringify({ front, back }),
    });

    if (!response.ok) {
        throw new Error("Failed to create flashcard");
    }

    return response.json();
}

export async function updateFlashcard(
    flashcardId: number,
    front: string,
    back: string
): Promise<void> {
    const response = await apiRequest(`/api/flashcards/${flashcardId}`, {
        method: "PUT",
        body: JSON.stringify({ front, back }),
    });

    if (!response.ok) {
        throw new Error("Failed to update flashcard");
    }
}

export async function deleteFlashcard(
    flashcardId: number,
): Promise<void> {
    const response = await apiRequest(`/api/flashcards/${flashcardId}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw new Error("Failed to delete flashcard");
    }    
}