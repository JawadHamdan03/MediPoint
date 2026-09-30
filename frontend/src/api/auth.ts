import { apiFetch } from "./client";
import type {
  DoctorSignUpDto,
  ForgotPasswordRequest,
  JwtTokenResponse,
  LoginRequest,
  MessageResponse,
  PatientSignUpDto,
  ResetPasswordRequest,
  Role,
  SignUpRole,
} from "../types/auth";

const rolePathSegment: Record<Role, string> = {
  Admin: "/api/Admin",
  Doctor: "/api/Doctor",
  Patient: "/patients",
};

export function login(role: Role, request: LoginRequest): Promise<JwtTokenResponse> {
  const path = role === "Admin" ? "/api/Admin/login" : role === "Doctor" ? "/api/Doctor/login" : "/patients/login";
  return apiFetch<JwtTokenResponse>(path, { method: "POST", body: request, auth: false });
}

export function forgotPassword(role: Role, request: ForgotPasswordRequest): Promise<MessageResponse> {
  return apiFetch<MessageResponse>(`${rolePathSegment[role]}/forgot-password`, { method: "POST", body: request, auth: false });
}

export function resetPassword(role: Role, request: ResetPasswordRequest): Promise<MessageResponse> {
  return apiFetch<MessageResponse>(`${rolePathSegment[role]}/reset-password`, { method: "POST", body: request, auth: false });
}

export function signUp(role: "Doctor", request: DoctorSignUpDto): Promise<JwtTokenResponse>;
export function signUp(role: "Patient", request: PatientSignUpDto): Promise<JwtTokenResponse>;
export function signUp(role: SignUpRole, request: DoctorSignUpDto | PatientSignUpDto): Promise<JwtTokenResponse> {
  const path = role === "Doctor" ? "/api/Doctor/sign-up" : "/patients/sign-up";
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
