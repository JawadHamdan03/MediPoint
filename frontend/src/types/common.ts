export type Gender = "Male" | "Female";

export type AppointmentStatus = "Pending" | "Confirmed" | "Completed" | "Cancelled";

export interface Medicine {
  id?: string;
  prescriptionId: string;
  patientId: string;
  name: string;
  dosage: string;
  frequency: string;
  durationDays: number;
  instructions: string;
}

export interface LabResult {
  id?: string;
  prescriptionId: string;
  patientId: string;
  testName: string;
  result: string;
  unit: string;
  referenceRange: string;
}
