import { apiFetch } from "./client";
import type {
  CancelAppointmentRequest,
  ChatRequest,
  ChatResult,
  DoctorResponse,
  MedicalRecordResponse,
  UpdatePatientDto,
} from "../types/patient";
import type { AppointmentResponse } from "../types/doctor";

export function searchDoctors(speciality: string): Promise<DoctorResponse[]> {
  return apiFetch<DoctorResponse[]>(`/patients/search-doctors/${encodeURIComponent(speciality)}`);
}

export function bookAppointment(appointmentId: string): Promise<AppointmentResponse> {
  return apiFetch<AppointmentResponse>(`/patients/book-appointment/${appointmentId}`, { method: "POST" });
}

export function getMedicalRecords(): Promise<MedicalRecordResponse[]> {
  return apiFetch<MedicalRecordResponse[]>("/patients/get-medical-records");
}

export function cancelAppointment(appointmentId: string, request: CancelAppointmentRequest) {
  return apiFetch<AppointmentResponse>(`/patients/cancel-appointment/${appointmentId}`, {
    method: "POST",
    body: request,
  });
}

export function updateDetails(details: UpdatePatientDto): Promise<UpdatePatientDto> {
  return apiFetch<UpdatePatientDto>("/patients/update-details", { method: "POST", body: details });
}

export function chat(request: ChatRequest): Promise<ChatResult> {
  return apiFetch<ChatResult>("/patients/chat", { method: "POST", body: request });
}
