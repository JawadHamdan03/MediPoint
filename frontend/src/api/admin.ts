import { apiFetch } from "./client";
import type { DoctorDto, PatientDto, UpdateDoctorDto } from "../types/admin";

export function addDoctor(doctor: DoctorDto): Promise<DoctorDto> {
  return apiFetch<DoctorDto>("/api/Admin/Add-doctor", { method: "POST", body: doctor });
}

export function updateDoctor(doctorId: string, doctor: UpdateDoctorDto): Promise<DoctorDto> {
  return apiFetch<DoctorDto>(`/api/Admin/update-doctor/${doctorId}`, { method: "POST", body: doctor });
}

export function removeDoctor(doctorId: string): Promise<DoctorDto> {
  return apiFetch<DoctorDto>(`/api/Admin/remove-doctor/${doctorId}`, { method: "POST" });
}

export function registerPatient(patient: PatientDto): Promise<PatientDto> {
  return apiFetch<PatientDto>("/api/Admin/register-patient", { method: "POST", body: patient });
}
