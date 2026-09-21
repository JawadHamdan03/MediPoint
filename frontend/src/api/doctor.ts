import { apiFetch } from "./client";
import type {
  ApponitmentDTO,
  AppointmentResponse,
  CompleteAppointmentRequest,
  PrescriptionRequest,
  PrescriptionResponse,
} from "../types/doctor";

export function getTodaysAppointments(): Promise<AppointmentResponse[]> {
  return apiFetch<AppointmentResponse[]>("/api/Doctor/get-Appointments-today");
}

export function addPrescription(request: PrescriptionRequest): Promise<PrescriptionResponse> {
  return apiFetch<PrescriptionResponse>("/api/Doctor/add-prescription", { method: "POST", body: request });
}

export function addAppointment(
  appointment: Pick<ApponitmentDTO, "appointmentDate" | "duration" | "doctorId">,
): Promise<ApponitmentDTO> {
  return apiFetch<ApponitmentDTO>("/api/Doctor/add-appointment", { method: "POST", body: appointment });
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
