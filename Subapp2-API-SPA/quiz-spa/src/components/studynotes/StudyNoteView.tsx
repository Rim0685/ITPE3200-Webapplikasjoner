import type { StudyNote } from "../../types/studyNotes";
import { formatLongDate } from "./formatDate";

interface StudyNoteViewProps {
    note: StudyNote;
    onEdit: () => void;
    onDelete: () => void;
}

// Shows the whole selected note in the reading pane
function StudyNoteView({ note, onEdit, onDelete }: StudyNoteViewProps) {
    return (
        <article className="note-view">
            <div className="note-view-toolbar">
                <span className="note-view-date">Created {formatLongDate(note.createdAt)}</span>
                <div className="note-view-actions">
                    <button type="button" className="btn btn-light" onClick={onEdit}>
                        Edit
                    </button>
                    <button type="button" className="btn btn-danger" onClick={onDelete}>
                        Delete
                    </button>
                </div>
            </div>

            <h1>{note.title}</h1>
            <hr />
            {/* pre-wrap in CSS keeps the line breaks the user typed */}
            <p className="note-view-content">{note.content}</p>
        </article>
    );
}

export default StudyNoteView;
