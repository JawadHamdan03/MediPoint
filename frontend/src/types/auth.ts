import type { Gender } from "./common";

export type Role = "Admin" | "Doctor" | "Patient";

export type SignUpRole = "Doctor" | "Patient";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface PatientSignUpDto {
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string; // yyyy-MM-dd
  gender: Gender;
  bloodType: string;
  address: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
}

export interface DoctorSignUpDto {
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string; // yyyy-MM-dd
  gender: Gender;
  specialty: string;
  licenseNumber: string;
  yearsOfExperience: number;
  consultationFee: number;
  biography: string;
}

export interface JwtTokenResponse {
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: string;
  userId: string;
  firstName: string | null;
  lastName: string | null;
  role: string | null;
  imageUrl: string | null;
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
