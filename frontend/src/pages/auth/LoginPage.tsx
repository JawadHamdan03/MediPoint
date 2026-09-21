import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { ProfileIcon, StethoscopeIcon, DashboardIcon } from "../../components/ui/icons";
import type { Role } from "../../types/auth";

const ROLES: { role: Role; icon: typeof ProfileIcon }[] = [
  { role: "Patient", icon: ProfileIcon },
  { role: "Doctor", icon: StethoscopeIcon },
  { role: "Admin", icon: DashboardIcon },
];

export default function LoginPage() {
  const auth = useAuth();
  const navigate = useNavigate();
  const [role, setRole] = useState<Role>("Patient");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSubmitting(true);
    try {
      await auth.login(role, { email, password });
      navigate(`/${role.toLowerCase()}`, { replace: true });
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div
      className="relative flex min-h-screen items-center justify-center overflow-hidden bg-(--bg) px-4"
      style={{ animation: "fade-in 0.2s ease-out" }}
    >
      <div
        aria-hidden
        className="pointer-events-none absolute -top-32 left-1/2 h-80 w-[36rem] -translate-x-1/2 rounded-full bg-(--accent) opacity-[0.12] blur-3xl"
      />
      <div className="relative w-full max-w-sm">
        <div className="mb-6 flex flex-col items-center gap-2 text-center">
          <span className="flex h-11 w-11 items-center justify-center rounded-(--radius) bg-(--accent) text-lg font-bold text-white shadow-(--shadow)">
            M
          </span>
          <h1 className="text-2xl font-semibold text-(--text-h)">MediPoint</h1>
          <p className="text-sm text-(--text)">Sign in to manage your appointments</p>
        </div>

        <Card>
          <div className="mb-5 flex rounded-(--radius) border border-(--border) bg-(--bg) p-1">
            {ROLES.map(({ role: r, icon: Icon }) => (
              <button
                key={r}
                type="button"
                onClick={() => setRole(r)}
                className={`flex flex-1 items-center justify-center gap-1.5 rounded-[calc(var(--radius)-4px)] px-3 py-1.5 text-sm font-medium transition-colors duration-150 ${
                  role === r ? "bg-(--accent) text-white shadow-sm" : "text-(--text) hover:bg-(--accent-bg)"
                }`}
              >
                <Icon className="h-4 w-4" />
                {r}
              </button>
            ))}
          </div>

          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              label="Email"
              type="email"
              autoComplete="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <Input
              label="Password"
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            {errors.length > 0 && <ErrorList errors={errors} />}

            <Button type="submit" disabled={submitting} className="w-full">
              {submitting ? "Signing in…" : `Sign in as ${role}`}
            </Button>
          </form>
        </Card>
      </div>
    </div>
  );
}
