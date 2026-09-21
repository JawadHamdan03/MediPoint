import type { AppointmentStatus, LabResult, Medicine } from "./common";

export interface AppointmentResponse {
  id: string;
  appointmentDate: string;
  duration: number;
  status: AppointmentStatus;
  reason?: string | null;
  notes?: string | null;
  patientId: string;
}

export interface PrescriptionRequest {
  notes: string;
  appointmentId: string;
  medicineName: string;
  dosage: string;
  frequency: string;
  durationDays: number;
  instructions: string;
  testName: string;
  result: string;
  unit: string;
  referenceRange: string;
}

export interface PrescriptionResponse {
  notes: string;
  medicines: Medicine[];
  labResults: LabResult[];
  patientId: string;
  appointmentId: string;
}

export interface ApponitmentDTO {
  id: string;
  appointmentDate: string;
  duration: number;
  doctorId: string;
  status: AppointmentStatus;
}

export interface CompleteAppointmentRequest {
  notes?: string | null;
}
