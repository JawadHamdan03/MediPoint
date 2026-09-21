export type Role = "Admin" | "Doctor" | "Patient";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface JwtTokenResponse {
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: string;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

// .NET writes the role claim using ClaimTypes.Role, which serializes to this
// long URI key in the JWT payload rather than a short "role" claim.
export const ROLE_CLAIM_KEY = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role";

export interface JwtClaims {
  sub: string;
  exp: number;
  [key: string]: unknown;
}
