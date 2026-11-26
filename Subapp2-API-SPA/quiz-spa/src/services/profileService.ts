import { apiRequest } from "./apiClient";
import type { Profile } from "../types/profile";

// GET /api/profile
export async function getProfile(): Promise<Profile> {
    const response = await apiRequest("/api/profile");

    if (!response.ok) {
        throw new Error("Could not load the profile.");
    }

    return response.json();
}

// PUT /api/profile
export async function updateProfile(
    firstName: string,
    lastName: string
): Promise<Profile> {
    const response = await apiRequest("/api/profile", {
        method: "PUT",
        body: JSON.stringify({ firstName, lastName }),
    });

    if (!response.ok) {
        throw new Error("Could not save the profile.");
    }

    return response.json();
}

// DELETE /api/profile (the password is sent in the body)
export async function deleteAccount(password: string): Promise<void> {
    const response = await apiRequest("/api/profile", {
        method: "DELETE",
        body: JSON.stringify({ password }),
    });

    // The API returns 400 when the password is wrong
    if (response.status === 400) {
        throw new Error("The password is incorrect.");
    }

    if (!response.ok) {
        throw new Error("Could not delete the account.");
    }
}