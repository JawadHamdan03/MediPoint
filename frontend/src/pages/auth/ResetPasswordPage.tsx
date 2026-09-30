import { useState, type FormEvent } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { resetPassword } from "../../api/auth";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { ThemeToggle } from "../../components/ui/ThemeToggle";
import type { Role } from "../../types/auth";

function isRole(value: string | null): value is Role {
  return value === "Admin" || value === "Doctor" || value === "Patient";
}

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const role = searchParams.get("role");
  const token = searchParams.get("token") ?? "";

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [done, setDone] = useState(false);

  const linkInvalid = !isRole(role) || !token;

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});

    if (newPassword !== confirmPassword) {
      setErrors(["Passwords do not match."]);
      return;
    }
    if (!isRole(role)) return;

    setSubmitting(true);
    try {
      await resetPassword(role, { token, newPassword });
      setDone(true);
      setTimeout(() => navigate("/login", { replace: true }), 2500);
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
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
          <h1 className="text-2xl font-semibold text-(--text-h)">Reset your password</h1>
          <p className="text-sm text-(--text)">Choose a new password for your account</p>
        </div>

        <Card>
          {linkInvalid ? (
            <div className="flex flex-col gap-4 text-center">
              <p className="text-sm text-(--text-h)">This reset link is invalid or incomplete.</p>
              <Link to="/forgot-password">
                <Button type="button" className="w-full">
                  Request a new link
                </Button>
              </Link>
            </div>
          ) : done ? (
            <p className="text-center text-sm text-(--text-h)">
              Your password has been reset. Redirecting you to sign in…
            </p>
          ) : (
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              <Input
                label="New password"
                type="password"
                autoComplete="new-password"
                required
                minLength={8}
                value={newPassword}
                error={fields.newpassword}
                onChange={(e) => setNewPassword(e.target.value)}
              />
              <Input
                label="Confirm new password"
                type="password"
                autoComplete="new-password"
                required
                minLength={8}
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />

              {errors.length > 0 && <ErrorList errors={errors} />}

              <Button type="submit" disabled={submitting} className="w-full">
                {submitting ? "Resetting…" : "Reset password"}
              </Button>
            </form>
          )}
        </Card>

        <p className="mt-4 text-center text-sm text-(--text)">
          <Link to="/login" className="font-medium text-(--accent)">
            Back to sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
