import type { StudyNote } from "../../types/studyNotes";
import { formatShortDate } from "./formatDate";

interface StudyNoteListItemProps {
    note: StudyNote;
    selected: boolean;
    onSelect: () => void;
}

// One row in the sidebar: title, date and the first line of the note
function StudyNoteListItem({ note, selected, onSelect }: StudyNoteListItemProps) {
    return (
        <button
            type="button"
            className={selected ? "note-item note-item-selected" : "note-item"}
            onClick={onSelect}
            aria-current={selected ? "true" : undefined}
        >
            <span className="note-item-top">
                <span className="note-item-title">{note.title}</span>
                <span className="note-item-date">{formatShortDate(note.createdAt)}</span>
            </span>
            {/* CSS cuts this to one line, so the list stays tidy */}
            <span className="note-item-preview">{note.content}</span>
        </button>
    );
}

export default StudyNoteListItem;
