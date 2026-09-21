import type { ProblemDetails } from "../types/auth";

const BASE_URL = import.meta.env.VITE_API_URL;

export class ApiError extends Error {
  status: number;
  problem: ProblemDetails;

  constructor(status: number, problem: ProblemDetails) {
    super(problem.detail ?? problem.title ?? `Request failed with status ${status}`);
    this.status = status;
    this.problem = problem;
  }
}

interface AuthHooks {
  getAccessToken: () => string | null;
  refreshAccessToken: () => Promise<string | null>;
  onAuthFailure: () => void;
}

// Set once by AuthContext on mount; keeps this module free of a circular
// import on the context while still letting apiFetch drive silent refresh.
let authHooks: AuthHooks | null = null;

export function configureApiClient(hooks: AuthHooks) {
  authHooks = hooks;
}

interface RequestOptions {
  method?: string;
  body?: unknown;
  /** Set false for login/refresh calls, which never carry or need a bearer token. */
  auth?: boolean;
  /** Set true when body is already a FormData instance (file uploads). */
  isForm?: boolean;
}

async function parseProblem(res: Response): Promise<ProblemDetails> {
  try {
    return (await res.json()) as ProblemDetails;
  } catch {
    return { title: res.statusText, status: res.status };
  }
}

async function rawFetch(path: string, options: RequestOptions, token: string | null): Promise<Response> {
  const headers: Record<string, string> = {};
  let body: BodyInit | undefined;

  if (options.isForm) {
    body = options.body as FormData;
  } else if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
    body = JSON.stringify(options.body);
  }

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return fetch(`${BASE_URL}${path}`, {
    method: options.method ?? (options.body !== undefined ? "POST" : "GET"),
    headers,
    body,
  });
}

export async function apiFetch<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const requiresAuth = options.auth !== false;
  const token = requiresAuth ? (authHooks?.getAccessToken() ?? null) : null;

  let res = await rawFetch(path, options, token);

  if (res.status === 401 && requiresAuth && authHooks) {
    const refreshed = await authHooks.refreshAccessToken();
    if (refreshed) {
      res = await rawFetch(path, options, refreshed);
    } else {
      authHooks.onAuthFailure();
    }
  }

  if (!res.ok) {
    throw new ApiError(res.status, await parseProblem(res));
  }

  if (res.status === 204) {
    return undefined as T;
  }

  return (await res.json()) as T;
}
