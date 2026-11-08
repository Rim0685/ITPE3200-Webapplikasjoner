import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/authService";


function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    // Handles login and stores the JWT token
    const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError("");

        try {
            const data = await login(email, password);

            localStorage.setItem("token", data.token);

            navigate("/");
        } catch {
            setError("Invalid email or password.");
        }
    };
    
    return (
        <div>
            <h1>Login</h1>

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="email">Email</label>
                    <input
                        id="email"
                        type="email"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        required
                    />
                </div>

                <div>
                    <label htmlFor="password">Password</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        required
                    />
                </div>

                {error && <p role="alert">{error}</p>}
                
                <button type="submit">Login</button>
            </form>
        </div>
    );
}

export default LoginPage;