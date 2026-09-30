import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { forgotPassword } from "../../api/auth";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { ProfileIcon, StethoscopeIcon, DashboardIcon } from "../../components/ui/icons";
import { ThemeToggle } from "../../components/ui/ThemeToggle";
import type { Role } from "../../types/auth";

const ROLES: { role: Role; icon: typeof ProfileIcon }[] = [
  { role: "Patient", icon: ProfileIcon },
  { role: "Doctor", icon: StethoscopeIcon },
  { role: "Admin", icon: DashboardIcon },
];

export default function ForgotPasswordPage() {
  const [role, setRole] = useState<Role>("Patient");
  const [email, setEmail] = useState("");
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSubmitting(true);
    try {
      const res = await forgotPassword(role, { email });
      setMessage(res.message);
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
      <ThemeToggle className="absolute top-4 right-4" />
      <div className="relative w-full max-w-sm">
        <div className="mb-6 flex flex-col items-center gap-2 text-center">
          <span className="flex h-11 w-11 items-center justify-center rounded-(--radius) bg-(--accent) text-lg font-bold text-white shadow-(--shadow)">
            M
          </span>
          <h1 className="text-2xl font-semibold text-(--text-h)">Forgot your password?</h1>
          <p className="text-sm text-(--text)">Enter your email and we'll send you a reset link</p>
        </div>

        <Card>
          {message ? (
            <div className="flex flex-col gap-4 text-center">
              <p className="text-sm text-(--text-h)">{message}</p>
              <Link to="/login">
                <Button type="button" className="w-full">
                  Back to sign in
                </Button>
              </Link>
            </div>
          ) : (
            <>
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

                {errors.length > 0 && <ErrorList errors={errors} />}

                <Button type="submit" disabled={submitting} className="w-full">
                  {submitting ? "Sending…" : "Send reset link"}
                </Button>
              </form>
            </>
          )}
        </Card>

        <p className="mt-4 text-center text-sm text-(--text)">
          Remembered your password?{" "}
          <Link to="/login" className="font-medium text-(--accent)">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
