import { apiFetch } from "./client";
import type { JwtTokenResponse, LoginRequest, Role } from "../types/auth";

export function login(role: Role, request: LoginRequest): Promise<JwtTokenResponse> {
  const path = role === "Admin" ? "/api/Admin/login" : role === "Doctor" ? "/api/Doctor/login" : "/patients/login";
  return apiFetch<JwtTokenResponse>(path, { method: "POST", body: request, auth: false });
}

export function refresh(role: Role, refreshToken: string): Promise<JwtTokenResponse> {
  if (role === "Admin") {
    return apiFetch<JwtTokenResponse>("/api/Admin/refreshtoken", { method: "POST", body: refreshToken, auth: false });
  }
  if (role === "Doctor") {
    return apiFetch<JwtTokenResponse>("/api/Doctor/refreshToken", { method: "POST", body: refreshToken, auth: false });
  }
  return apiFetch<JwtTokenResponse>("/patients/refresh-token", {
    method: "POST",
    body: { refreshToken },
    auth: false,
  });
}
