// Base URL for the ASP.NET Core API
const API_BASE_URL = "https://localhost:7262";

export async function apiRequest(
    endpoint: string,
    options: RequestInit = {}
): Promise<Response> {
    const token = localStorage.getItem("token");

    // Add the JWT token when the user is logged in
    const headers = new Headers(options.headers);

    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    // Set JSON content type only when sending a request body
    if (options.body) {
        headers.set("Content-Type", "application/json");
    }

    return fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers,
    });
}