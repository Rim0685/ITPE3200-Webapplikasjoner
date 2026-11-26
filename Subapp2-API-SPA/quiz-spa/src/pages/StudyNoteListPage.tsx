import { useEffect, useState } from "react";
import type { StudyNote } from "../types/studyNotes";
import {
    createStudyNote,
    deleteStudyNote,
    getStudyNotes,
    updateStudyNote
} from "../services/studyNoteService";
import StudyNoteForm from "../components/studynotes/StudyNoteForm";
import StudyNoteListItem from "../components/studynotes/StudyNoteListItem";
import StudyNoteView from "../components/studynotes/StudyNoteView";
import ConfirmDeleteDialog from "../components/studynotes/ConfirmDeleteDialog";

// What the reading pane on the right is showing
type PaneMode = "view" | "edit" | "new";

// Page where the logged-in user can create, read, edit and delete their study notes.
// The layout is inspired by Obsidian: a list on the left and the selected note on the right.
function StudyNoteListPage() {
    const [notes, setNotes] = useState<StudyNote[]>([]);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const [mode, setMode] = useState<PaneMode>("view");
    const [search, setSearch] = useState("");
    // The note waiting for delete confirmation, or null when the dialog is closed
    const [noteToDelete, setNoteToDelete] = useState<StudyNote | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Loads the notes once when the page opens, and selects the newest one
    useEffect(() => {
        async function loadNotes() {
            try {
                const data = await getStudyNotes();
                setNotes(data);
                setSelectedId(data.length > 0 ? data[0].studyNoteId : null);
            } catch {
                setError("Could not load study notes.");
            } finally {
                setLoading(false);
            }
        }

        loadNotes();
    }, []);

    // Fetches the notes again after a change, and selects the given note
    const refreshNotes = async (selectId: number | null) => {
        try {
            const data = await getStudyNotes();
            setNotes(data);
            setSelectedId(selectId ?? (data.length > 0 ? data[0].studyNoteId : null));
            setError("");
        } catch {
            setError("Could not refresh study notes.");
        }
    };

    const selectedNote = notes.find((note) => note.studyNoteId === selectedId) ?? null;

    // Search filters the notes already loaded, so it needs no extra API call
    const searchText = search.trim().toLowerCase();
    const visibleNotes = searchText
        ? notes.filter((note) =>
              note.title.toLowerCase().includes(searchText) ||
              note.content.toLowerCase().includes(searchText))
        : notes;

    const handleSelect = (noteId: number) => {
        setSelectedId(noteId);
        setMode("view");
    };

    // Runs when the user confirms in the dialog
    const handleConfirmDelete = async () => {
        if (!noteToDelete) {
            return;
        }

        try {
            await deleteStudyNote(noteToDelete.studyNoteId);
            await refreshNotes(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Could not delete the note");
        } finally {
            setNoteToDelete(null);
        }
    };

    if (loading) {
        return <p className="container">Loading study notes...</p>;
    }

    return (
        <div className="container notes-page">
            <div className="notes-layout">
                <aside className="notes-sidebar">
                    <div className="notes-sidebar-header">
                        <h2>My notes</h2>

                        <label className="notes-search">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none"
                                stroke="currentColor" strokeWidth="2" strokeLinecap="round"
                                aria-hidden="true">
                                <circle cx="11" cy="11" r="7" />
                                <path d="M20 20l-3.5-3.5" />
                            </svg>
                            <input
                                type="search"
                                placeholder="Search notes..."
                                aria-label="Search notes"
                                value={search}
                                onChange={(event) => setSearch(event.target.value)}
                            />
                        </label>

                        <button
                            type="button"
                            className="btn note-btn-primary notes-new-button"
                            onClick={() => setMode("new")}
                        >
                            + New note
                        </button>
                    </div>

                    <nav className="notes-list" aria-label="Study notes">
                        {notes.length === 0 && (
                            <p className="notes-list-empty">No notes yet.</p>
                        )}
                        {notes.length > 0 && visibleNotes.length === 0 && (
                            <p className="notes-list-empty">No notes match "{search}".</p>
                        )}
                        {visibleNotes.map((note) => (
                            <StudyNoteListItem
                                key={note.studyNoteId}
                                note={note}
                                selected={note.studyNoteId === selectedId && mode !== "new"}
                                onSelect={() => handleSelect(note.studyNoteId)}
                            />
                        ))}
                    </nav>
                </aside>

                <main className="notes-pane">
                    {error && <p role="alert" className="note-error">{error}</p>}

                    {mode === "new" && (
                        <StudyNoteForm
                            submitLabel="Add note"
                            onCancel={() => setMode("view")}
                            onSubmit={async (title, content) => {
                                const created = await createStudyNote(title, content);
                                setMode("view");
                                await refreshNotes(created.studyNoteId);
                            }}
                        />
                    )}

                    {/* key resets the form when another note is edited */}
                    {mode === "edit" && selectedNote && (
                        <StudyNoteForm
                            key={selectedNote.studyNoteId}
                            initialTitle={selectedNote.title}
                            initialContent={selectedNote.content}
                            submitLabel="Save changes"
                            onCancel={() => setMode("view")}
                            onSubmit={async (title, content) => {
                                await updateStudyNote(selectedNote.studyNoteId, title, content);
                                setMode("view");
                                await refreshNotes(selectedNote.studyNoteId);
                            }}
                        />
                    )}

                    {mode === "view" && selectedNote && (
                        <StudyNoteView
                            note={selectedNote}
                            onEdit={() => setMode("edit")}
                            onDelete={() => setNoteToDelete(selectedNote)}
                        />
                    )}

                    {mode === "view" && !selectedNote && (
                        <div className="notes-pane-empty">
                            <strong>No note selected</strong>
                            <p>Pick a note from the list, or create a new one.</p>
                        </div>
                    )}
                </main>
            </div>

            <ConfirmDeleteDialog
                noteTitle={noteToDelete?.title ?? null}
                onConfirm={handleConfirmDelete}
                onCancel={() => setNoteToDelete(null)}
            />
        </div>
    );
}

export default StudyNoteListPage;
