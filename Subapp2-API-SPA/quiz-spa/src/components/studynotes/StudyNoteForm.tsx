import { useState } from "react";

const MaxTitleLength = 100;
const MaxContentLength = 5000;

interface StudyNoteFormProps {
    initialTitle?: string;
    initialContent?: string;
    submitLabel: string;
    onSubmit: (title: string, content: string) => Promise<void>;
    onCancel: () => void;
}

// Shared form used both for creating a new note and editing an existing one
function StudyNoteForm({
    initialTitle = "",
    initialContent = "",
    submitLabel,
    onSubmit,
    onCancel
}: StudyNoteFormProps) {
    const [title, setTitle] = useState(initialTitle);
    const [content, setContent] = useState(initialContent);
    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError("");

        // Same rule as the API: only spaces counts as empty
        if (!title.trim() || !content.trim()) {
            setError("Both title and note are required.");
            return;
        }

        setSaving(true);

        try {
            await onSubmit(title.trim(), content.trim());
        } catch (err) {
            setError(err instanceof Error ? err.message : "Could not save the note");
            setSaving(false);
        }
    };

    return (
        <form className="note-form" onSubmit={handleSubmit}>
            {/* Large title field, so the form looks like the note it becomes */}
            <input
                className="note-form-title"
                type="text"
                aria-label="Title"
                placeholder="Title"
                maxLength={MaxTitleLength}
                value={title}
                onChange={(event) => setTitle(event.target.value)}
                autoFocus
                required
            />

            <textarea
                className="note-form-content"
                aria-label="Note"
                placeholder="Write what you want to remember..."
                maxLength={MaxContentLength}
                value={content}
                onChange={(event) => setContent(event.target.value)}
                required
            />

            {error && <p role="alert" className="note-error">{error}</p>}

            <div className="note-form-footer">
                {/* Shows how close the user is to the limit */}
                <span className="note-char-count">
                    {content.length} / {MaxContentLength}
                </span>
                <button type="button" className="btn btn-light" onClick={onCancel}>
                    Cancel
                </button>
                <button type="submit" className="btn note-btn-primary" disabled={saving}>
                    {saving ? "Saving..." : submitLabel}
                </button>
            </div>
        </form>
    );
}

export default StudyNoteForm;
