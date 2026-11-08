import { apiRequest } from "./apiClient";

export interface LoginResponse {
    token: string;
    email: string;
    expiresAt: string;
}

export async function login(
    email: string,
    password: string
): Promise<LoginResponse> {
    const response = await apiRequest("/api/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        throw new Error("Login failed");
    }

    return response.json();
}

export async function register(
    email: string,
    password: string
): Promise<void> {
    const response = await apiRequest("/api/auth/register", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        throw new Error("Registration failed");
    }
}