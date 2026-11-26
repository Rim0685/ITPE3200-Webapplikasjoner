import { useEffect, useRef, useState } from "react";

interface ConfirmDeleteDialogProps {
    // The dialog is shown when a title is given, and hidden when it is null
    noteTitle: string | null;
    onConfirm: () => Promise<void>;
    onCancel: () => void;
}

// Replaces window.confirm with a dialog that matches the rest of the page.
// The built-in <dialog> element handles Esc, keyboard focus and the dark backdrop.
function ConfirmDeleteDialog({ noteTitle, onConfirm, onCancel }: ConfirmDeleteDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [deleting, setDeleting] = useState(false);

    // Opens or closes the dialog when noteTitle changes
    useEffect(() => {
        const dialog = dialogRef.current;

        if (!dialog) {
            return;
        }

        if (noteTitle !== null && !dialog.open) {
            dialog.showModal();
        } else if (noteTitle === null && dialog.open) {
            dialog.close();
        }
    }, [noteTitle]);

    const handleConfirm = async () => {
        setDeleting(true);

        try {
            await onConfirm();
        } finally {
            setDeleting(false);
        }
    };

    return (
        <dialog
            ref={dialogRef}
            className="note-dialog"
            aria-labelledby="note-dialog-title"
            // Esc fires "cancel"; preventDefault lets React decide when to close
            onCancel={(event) => {
                event.preventDefault();
                onCancel();
            }}
        >
            <div className="note-dialog-icon" aria-hidden="true">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                    strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M4 7h16M10 11v6M14 11v6M6 7l1 13h10l1-13M9 7V4h6v3" />
                </svg>
            </div>

            <h2 id="note-dialog-title">Delete note?</h2>
            <p>
                <strong>"{noteTitle}"</strong> will be deleted permanently.
                This cannot be undone.
            </p>

            <div className="note-dialog-actions">
                <button type="button" className="btn btn-light" onClick={onCancel} autoFocus>
                    Cancel
                </button>
                <button
                    type="button"
                    className="btn note-btn-danger"
                    onClick={handleConfirm}
                    disabled={deleting}
                >
                    {deleting ? "Deleting..." : "Delete"}
                </button>
            </div>
        </dialog>
    );
}

export default ConfirmDeleteDialog;
