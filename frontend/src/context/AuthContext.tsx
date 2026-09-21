import { createContext, useContext, useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { configureApiClient } from "../api/client";
import { login as apiLogin, refresh as apiRefresh } from "../api/auth";
import type { JwtClaims, LoginRequest, Role } from "../types/auth";

const ACTIVE_ROLE_KEY = "medipoint_active_role";
const refreshStorageKey = (role: Role) => `medipoint_refresh_${role}`;

function decodeJwt(token: string): JwtClaims {
  const base64Url = token.split(".")[1];
  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(base64Url.length / 4) * 4, "=");
  const json = decodeURIComponent(
    atob(base64)
      .split("")
      .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
      .join(""),
  );
  return JSON.parse(json) as JwtClaims;
}

interface AuthState {
  accessToken: string | null;
  role: Role | null;
  userId: string | null;
}

type AuthStatus = "loading" | "authenticated" | "unauthenticated";

interface AuthContextValue extends AuthState {
  status: AuthStatus;
  login: (role: Role, request: LoginRequest) => Promise<void>;
  logout: () => void;
}

const emptyState: AuthState = { accessToken: null, role: null, userId: null };

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(emptyState);
  const [status, setStatus] = useState<AuthStatus>("loading");
  // apiFetch's hooks are configured once and close over stale state, so
  // mirror the latest values into a ref they can read at call time.
  const stateRef = useRef(state);

  function applyToken(role: Role, accessToken: string, refreshToken: string | null) {
    const claims = decodeJwt(accessToken);
    const next: AuthState = { accessToken, role, userId: claims.sub };
    stateRef.current = next;
    setState(next);
    localStorage.setItem(ACTIVE_ROLE_KEY, role);
    if (refreshToken) {
      localStorage.setItem(refreshStorageKey(role), refreshToken);
    }
    setStatus("authenticated");
  }

  function clearAuth(role: Role | null) {
    stateRef.current = emptyState;
    setState(emptyState);
    if (role) {
      localStorage.removeItem(refreshStorageKey(role));
    }
    localStorage.removeItem(ACTIVE_ROLE_KEY);
    setStatus("unauthenticated");
  }

  async function login(role: Role, request: LoginRequest) {
    const res = await apiLogin(role, request);
    applyToken(role, res.accessToken!, res.refreshToken);
  }

  function logout() {
    clearAuth(stateRef.current.role);
  }

  useEffect(() => {
    configureApiClient({
      getAccessToken: () => stateRef.current.accessToken,
      refreshAccessToken: async () => {
        const role = stateRef.current.role ?? (localStorage.getItem(ACTIVE_ROLE_KEY) as Role | null);
        if (!role) return null;
        const stored = localStorage.getItem(refreshStorageKey(role));
        if (!stored) return null;
        try {
          const res = await apiRefresh(role, stored);
          applyToken(role, res.accessToken!, res.refreshToken);
          return res.accessToken;
        } catch {
          clearAuth(role);
          return null;
        }
      },
      onAuthFailure: () => clearAuth(stateRef.current.role),
    });

    const role = localStorage.getItem(ACTIVE_ROLE_KEY) as Role | null;
    const stored = role ? localStorage.getItem(refreshStorageKey(role)) : null;
    if (role && stored) {
      apiRefresh(role, stored)
        .then((res) => applyToken(role, res.accessToken!, res.refreshToken))
        .catch(() => clearAuth(role));
    } else {
      setStatus("unauthenticated");
    }
    // Runs once: apiFetch's auth hooks and the bootstrap refresh both only
    // need to happen on initial mount.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const value = useMemo<AuthContextValue>(() => ({ ...state, status, login, logout }), [state, status]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return ctx;
}
