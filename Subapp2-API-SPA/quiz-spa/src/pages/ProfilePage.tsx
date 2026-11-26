import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import {
    deleteAccount,
    getProfile,
    updateProfile,
} from "../services/profileService";

function ProfilePage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);

    // Delete account state
    const [showDelete, setShowDelete] = useState(false);
    const [password, setPassword] = useState("");
    const [deleteError, setDeleteError] = useState<string | null>(null);

    useEffect(() => {
        const loadProfile = async () => {
            try {
                const profile = await getProfile();
                setEmail(profile.email);
                setFirstName(profile.firstName);
                setLastName(profile.lastName);
            } catch {
                setError("Could not load the profile.");
            } finally {
                setLoading(false);
            }
        };

        loadProfile();
    }, []);

    const handleSave = async (e: FormEvent) => {
        e.preventDefault();
        setError(null);
        setMessage(null);

        if (!firstName.trim() || !lastName.trim()) {
            setError("First name and last name are required.");
            return;
        }

        try {
            const profile = await updateProfile(firstName, lastName);
            setFirstName(profile.firstName);
            setLastName(profile.lastName);
            setMessage("Your profile was saved.");
        } catch {
            setError("Could not save the profile.");
        }
    };

    const handleDelete = async (e: FormEvent) => {
        e.preventDefault();
        setDeleteError(null);

        try {
            await deleteAccount(password);
            // The account is gone, so the token is no longer valid
            localStorage.removeItem("token");
            navigate("/register");
        } catch (err) {
            setDeleteError(
                err instanceof Error ? err.message : "Could not delete the account."
            );
        }
    };

    if (loading) {
        return <div className="container" role="status">Loading profile...</div>;
    }

    return (
        <div className="container">
            <h1>My profile</h1>
            <p className="muted">Change your name or delete your account.</p>

            <div className="form-container">
                <form onSubmit={handleSave}>
                    <label htmlFor="email">Email</label>
                    <input id="email" type="email" value={email} disabled />

                    <label htmlFor="firstName">First name</label>
                    <input
                        id="firstName"
                        type="text"
                        value={firstName}
                        maxLength={100}
                        onChange={(e) => setFirstName(e.target.value)}
                    />

                    <label htmlFor="lastName">Last name</label>
                    <input
                        id="lastName"
                        type="text"
                        value={lastName}
                        maxLength={100}
                        onChange={(e) => setLastName(e.target.value)}
                    />

                    {error && <p role="alert">{error}</p>}
                    {message && <p role="status">{message}</p>}

                    <button type="submit" className="btn btn-dark">
                        Save
                    </button>
                </form>
            </div>

            <div className="form-container">
                <h3>Delete account</h3>
                <p className="muted">
                    This deletes your account, flashcards, decks and quiz attempts. It
                    cannot be undone.
                </p>

                {!showDelete ? (
                    <button
                        type="button"
                        className="btn btn-danger"
                        onClick={() => setShowDelete(true)}
                    >
                        Delete my account
                    </button>
                ) : (
                    <form onSubmit={handleDelete}>
                        <label htmlFor="password">Confirm with your password</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />

                        {deleteError && <p role="alert">{deleteError}</p>}

                        <button type="submit" className="btn btn-danger">
                            Delete permanently
                        </button>
                        <button
                            type="button"
                            className="btn btn-light"
                            onClick={() => {
                                setShowDelete(false);
                                setPassword("");
                                setDeleteError(null);
                            }}
                        >
                            Cancel
                        </button>
                    </form>
                )}
            </div>
        </div>
    );
}

export default ProfilePage;