import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { ApiError } from "../../api/client";
import type { Role } from "../../types/auth";

const ROLES: Role[] = ["Patient", "Doctor", "Admin"];

export default function LoginPage() {
  const auth = useAuth();
  const navigate = useNavigate();
  const [role, setRole] = useState<Role>("Patient");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await auth.login(role, { email, password });
      navigate(`/${role.toLowerCase()}`, { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Something went wrong. Try again.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-sm flex-col justify-center gap-6 px-4">
      <div>
        <h1 className="text-(length:--tw-heading,2rem) text-3xl font-semibold text-(--text-h)">MediPoint</h1>
        <p className="mt-1 text-sm text-(--text)">Sign in to continue</p>
      </div>

      <div className="flex rounded-lg border border-(--border) p-1">
        {ROLES.map((r) => (
          <button
            key={r}
            type="button"
            onClick={() => setRole(r)}
            className={`flex-1 rounded-md px-3 py-1.5 text-sm font-medium transition-colors ${
              role === r ? "bg-(--accent) text-white" : "text-(--text) hover:bg-(--accent-bg)"
            }`}
          >
            {r}
          </button>
        ))}
      </div>

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <label className="flex flex-col gap-1 text-sm text-(--text)">
          Email
          <input
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="rounded-md border border-(--border) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent)"
          />
        </label>

        <label className="flex flex-col gap-1 text-sm text-(--text)">
          Password
          <input
            type="password"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="rounded-md border border-(--border) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent)"
          />
        </label>

        {error && <p className="text-sm text-red-500">{error}</p>}

        <button
          type="submit"
          disabled={submitting}
          className="rounded-md bg-(--accent) px-3 py-2 text-sm font-medium text-white disabled:opacity-60"
        >
          {submitting ? "Signing in…" : "Sign in"}
        </button>
      </form>
    </div>
  );
}
