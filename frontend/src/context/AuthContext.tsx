import { createContext, useContext, useEffect, useRef, useState, type ReactNode } from "react";
import { configureApiClient } from "../api/client";
import { login as apiLogin, refresh as apiRefresh } from "../api/auth";
import type { JwtClaims, LoginRequest, Role } from "../types/auth";

const ACTIVE_ROLE_KEY = "medipoint_active_role";
const refreshStorageKey = (role: Role) => `medipoint_refresh_${role}`;

function getStoredSession(): { role: Role; token: string } | null {
  const role = localStorage.getItem(ACTIVE_ROLE_KEY) as Role | null;
  if (!role) return null;
  const token = localStorage.getItem(refreshStorageKey(role));
  return token ? { role, token } : null;
}

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
  // A stored session needs an async refresh before we know if it's valid, so
  // only that case starts in "loading"; everything else is known synchronously.
  const [status, setStatus] = useState<AuthStatus>(() => (getStoredSession() ? "loading" : "unauthenticated"));
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

    const session = getStoredSession();
    if (session) {
      apiRefresh(session.role, session.token)
        .then((res) => applyToken(session.role, res.accessToken!, res.refreshToken))
        .catch(() => clearAuth(session.role));
    }
    // Runs once: apiFetch's auth hooks and the bootstrap refresh both only
    // need to happen on initial mount.
  }, []);

  const value: AuthContextValue = { ...state, status, login, logout };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components -- small app: co-locating the hook with its provider is worth the fast-refresh tradeoff.
export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return ctx;
}
