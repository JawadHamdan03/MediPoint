import type { AppointmentStatus, Gender, LabResult, Medicine } from "./common";

export interface AppointmentDTO {
  id: string;
  appointmentDate: string;
  duration: number;
  status: AppointmentStatus;
}

export interface DoctorResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  specialty: string;
  licenseNumber: string;
  yearsOfExperience: number;
  consultationFee: number;
  biography: string;
  imageUrl?: string | null;
  averageRating: number | null;
  reviewCount: number;
  appointmentDTOs: AppointmentDTO[];
}

export interface ReviewResponse {
  id: string;
  rating: number;
  comment: string | null;
  createdAt: string;
  patientName: string;
}

export interface MyAppointmentResponse {
  id: string;
  appointmentDate: string;
  duration: number;
  status: AppointmentStatus;
  reason: string | null;
  notes: string | null;
  cancellationReason: string | null;
  doctorId: string;
  doctorName: string;
  hasReview: boolean;
}

export interface AddReviewRequest {
  rating: number;
  comment?: string | null;
}

export interface MedicalRecordResponse {
  prescriptionId: string;
  diagnosis: string;
  notes: string;
  medicines: Medicine[];
  labResults: LabResult[];
  doctorName: string;
}

export interface CancelAppointmentRequest {
  cancellationReason?: string | null;
}

export interface UpdatePatientDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: Gender;
  bloodType: string;
  address: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
}

export interface ChatRequest {
  message: string;
}

export interface ChatResult {
  reply: string;
}
