import { apiFetch } from "./client";
import type {
  ApponitmentDTO,
  AppointmentResponse,
  CompleteAppointmentRequest,
  GenerateAppointmentSlotsRequest,
  GenerateAppointmentSlotsResult,
  PrescriptionRequest,
  PrescriptionResponse,
  ReviewResponse,
} from "../types/doctor";

export function getTodaysAppointments(): Promise<AppointmentResponse[]> {
  return apiFetch<AppointmentResponse[]>("/api/Doctor/get-Appointments-today");
}

/** @param date Optional yyyy-MM-dd filter; omit to fetch every appointment for the doctor. */
export function getAppointments(date?: string): Promise<AppointmentResponse[]> {
  return apiFetch<AppointmentResponse[]>(`/api/Doctor/appointments${date ? `?date=${date}` : ""}`);
}

export function addPrescription(request: PrescriptionRequest): Promise<PrescriptionResponse> {
  return apiFetch<PrescriptionResponse>("/api/Doctor/add-prescription", { method: "POST", body: request });
}

export function addAppointment(
  appointment: Pick<ApponitmentDTO, "appointmentDate" | "duration" | "doctorId">,
): Promise<ApponitmentDTO> {
  return apiFetch<ApponitmentDTO>("/api/Doctor/add-appointment", { method: "POST", body: appointment });
}

export function generateAppointmentSlots(
  request: GenerateAppointmentSlotsRequest,
): Promise<GenerateAppointmentSlotsResult> {
  return apiFetch<GenerateAppointmentSlotsResult>("/api/Doctor/generate-slots", { method: "POST", body: request });
}

export function getMyReviews(): Promise<ReviewResponse[]> {
  return apiFetch<ReviewResponse[]>("/api/Doctor/reviews");
}

export function completeAppointment(
  appointmentId: string,
  request: CompleteAppointmentRequest,
): Promise<AppointmentResponse> {
  return apiFetch<AppointmentResponse>(`/api/Doctor/complete-appointment/${appointmentId}`, {
    method: "POST",
    body: request,
  });
}
