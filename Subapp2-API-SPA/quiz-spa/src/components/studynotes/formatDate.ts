// Date helpers for study notes, so the list and the reading pane format dates the same way

// E.g. "18 Nov"
export function formatShortDate(value: string): string {
    return new Date(value).toLocaleDateString("en-GB", { day: "numeric", month: "short" });
}

// E.g. "18 Nov 2026"
export function formatLongDate(value: string): string {
    return new Date(value).toLocaleDateString("en-GB", {
        day: "numeric",
        month: "short",
        year: "numeric"
    });
}
