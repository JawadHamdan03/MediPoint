import type { AppointmentStatus, LabResult, Medicine } from "./common";

export interface AppointmentResponse {
  id: string;
  appointmentDate: string;
  duration: number;
  status: AppointmentStatus;
  reason?: string | null;
  notes?: string | null;
  patientId: string | null;
}

export interface PrescriptionRequest {
  diagnosis: string;
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
  diagnosis: string;
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

export type DayOfWeekName = "Sunday" | "Monday" | "Tuesday" | "Wednesday" | "Thursday" | "Friday" | "Saturday";

export interface GenerateAppointmentSlotsRequest {
  startDate: string; // yyyy-MM-dd
  endDate: string; // yyyy-MM-dd
  daysOfWeek: DayOfWeekName[];
  startTime: string; // HH:mm
  endTime: string; // HH:mm
  slotDurationMinutes: number;
}

export interface GenerateAppointmentSlotsResult {
  created: number;
  skipped: number;
  slots: ApponitmentDTO[];
}

export interface ReviewResponse {
  id: string;
  rating: number;
  comment: string | null;
  createdAt: string;
  patientName: string;
}
