import { apiRequest } from "./apiClient";
import type { FlashcardDeck } from "../types/flashcards";

// Uses the API's error text (e.g. "deck limit reached") when there is one
async function toError(response: Response, fallback: string): Promise<Error> {
    const text = await response.text();

    return new Error(text && !text.startsWith("{") ? text : fallback);
}

export async function getDecks(): Promise<FlashcardDeck[]> {
    const response = await apiRequest("/api/flashcarddecks");

    if (!response.ok) {
        throw await toError(response, "Failed to fetch decks");
    }

    return response.json();
}

export async function getDeck(deckId: number): Promise<FlashcardDeck> {
    const response = await apiRequest(`/api/flashcarddecks/${deckId}`);

    if (!response.ok) {
        throw await toError(response, "Failed to fetch the deck");
    }

    return response.json();
}

export async function createDeck(
    name: string,
    description: string | null
): Promise<FlashcardDeck> {
    const response = await apiRequest("/api/flashcarddecks", {
        method: "POST",
        body: JSON.stringify({ name, description }),
    });

    if (!response.ok) {
        throw await toError(response, "Failed to create the deck");
    }

    return response.json();
}

export async function updateDeck(
    deckId: number,
    name: string,
    description: string | null
): Promise<void> {
    const response = await apiRequest(`/api/flashcarddecks/${deckId}`, {
        method: "PUT",
        body: JSON.stringify({ name, description }),
    });

    if (!response.ok) {
        throw await toError(response, "Failed to update the deck");
    }
}

export async function deleteDeck(deckId: number): Promise<void> {
    const response = await apiRequest(`/api/flashcarddecks/${deckId}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw await toError(response, "Failed to delete the deck");
    }
}