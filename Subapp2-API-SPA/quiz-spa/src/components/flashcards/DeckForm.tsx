import { useState } from "react";

interface DeckFormProps {
    initialName?: string;
    initialDescription?: string;
    submitLabel: string;
    // Clears the fields after a successful save (used when creating)
    resetAfterSubmit?: boolean;
    onSubmit: (name: string, description: string) => Promise<void>;
}

function DeckForm({
    initialName = "",
    initialDescription = "",
    submitLabel,
    resetAfterSubmit = false,
    onSubmit
}: DeckFormProps) {
    const [name, setName] = useState(initialName);
    const [description, setDescription] = useState(initialDescription);
    const [error, setError] = useState("");

    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError("");

        try {
            await onSubmit(name.trim(), description.trim());

            if (resetAfterSubmit) {
                setName("");
                setDescription("");
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : "Could not save the deck");
        }
    };

    return (
        <form className="flashcard-form card" onSubmit={handleSubmit}>
            <div>
                <label htmlFor="deck-name">Name</label>
                <input
                    id="deck-name"
                    type="text"
                    maxLength={100}
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                    required
                />
            </div>

            <div>
                <label htmlFor="deck-description">Description (optional)</label>
                <input
                    id="deck-description"
                    type="text"
                    maxLength={500}
                    value={description}
                    onChange={(event) => setDescription(event.target.value)}
                />
            </div>

            {error && <p role="alert">{error}</p>}

            <button type="submit" className="btn btn-dark">
                {submitLabel}
            </button>
        </form>
    );
}

export default DeckForm;