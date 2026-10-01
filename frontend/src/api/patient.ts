import { apiFetch, apiFetchBlob } from "./client";
import type {
  AddReviewRequest,
  CancelAppointmentRequest,
  ChatRequest,
  ChatResult,
  DoctorResponse,
  MedicalRecordResponse,
  MyAppointmentResponse,
  ReviewResponse,
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

export function getMyAppointments(): Promise<MyAppointmentResponse[]> {
  return apiFetch<MyAppointmentResponse[]>("/patients/appointments");
}

export function addReview(appointmentId: string, request: AddReviewRequest): Promise<ReviewResponse> {
  return apiFetch<ReviewResponse>(`/patients/appointments/${appointmentId}/review`, { method: "POST", body: request });
}

export function getDoctorReviews(doctorId: string): Promise<ReviewResponse[]> {
  return apiFetch<ReviewResponse[]>(`/patients/doctors/${doctorId}/reviews`);
}

export function getPrescriptionPdf(prescriptionId: string): Promise<Blob> {
  return apiFetchBlob(`/patients/prescriptions/${prescriptionId}/pdf`);
}
