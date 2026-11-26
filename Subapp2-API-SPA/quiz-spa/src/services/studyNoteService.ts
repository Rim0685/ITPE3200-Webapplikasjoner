import { apiRequest } from "./apiClient";
import type { StudyNote } from "../types/studyNotes";

// Uses the API's error text (e.g. "The title is required.") when there is one
async function toError(response: Response, fallback: string): Promise<Error> {
    const text = await response.text();

    // Validation errors (400) come as JSON with an "errors" object
    if (text.startsWith("{")) {
        try {
            const problem = JSON.parse(text);
            const messages = Object.values(problem.errors ?? {}).flat();

            if (messages.length > 0) {
                return new Error(messages.join(" "));
            }
        } catch {
            // Falls back to the general message below
        }

        return new Error(fallback);
    }

    return new Error(text || fallback);
}

export async function getStudyNotes(): Promise<StudyNote[]> {
    const response = await apiRequest("/api/studynotes");

    if (!response.ok) {
        throw await toError(response, "Failed to fetch study notes");
    }

    return response.json();
}

export async function createStudyNote(
    title: string,
    content: string
): Promise<StudyNote> {
    const response = await apiRequest("/api/studynotes", {
        method: "POST",
        body: JSON.stringify({ title, content }),
    });

    if (!response.ok) {
        throw await toError(response, "Failed to create the study note");
    }

    return response.json();
}

export async function updateStudyNote(
    studyNoteId: number,
    title: string,
    content: string
): Promise<void> {
    const response = await apiRequest(`/api/studynotes/${studyNoteId}`, {
        method: "PUT",
        body: JSON.stringify({ title, content }),
    });

    if (!response.ok) {
        throw await toError(response, "Failed to update the study note");
    }
}

export async function deleteStudyNote(studyNoteId: number): Promise<void> {
    const response = await apiRequest(`/api/studynotes/${studyNoteId}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw await toError(response, "Failed to delete the study note");
    }
}
