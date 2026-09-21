import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import type { Role } from "../types/auth";

export function ProtectedRoute({ role }: { role: Role }) {
  const auth = useAuth();

  if (auth.status === "loading") {
    return <div className="flex h-screen items-center justify-center text-(--text)">Loading…</div>;
  }

  if (auth.status === "unauthenticated" || auth.role !== role) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
