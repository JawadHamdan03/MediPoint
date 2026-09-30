import type { Gender } from "./common";

export interface DoctorDto {
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
  isAvailable: boolean;
}

export interface UpdateDoctorDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: Gender;
  specialty: string;
  licenseNumber: string;
  yearsOfExperience: number;
  consultationFee: number;
  biography: string;
  isAvailable: boolean;
}

export interface PatientDto {
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: Gender;
  bloodType: string;
  address: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
}

export interface SpecialtyRevenue {
  specialty: string;
  revenue: number;
  completedAppointments: number;
}

export interface DailyAppointmentCount {
  date: string; // yyyy-MM-dd
  count: number;
}

export interface TopRatedDoctor {
  doctorId: string;
  name: string;
  specialty: string;
  averageRating: number;
  reviewCount: number;
}

export interface AdminDashboardResponse {
  totalAppointments: number;
  pendingCount: number;
  confirmedCount: number;
  completedCount: number;
  cancelledCount: number;
  missedCount: number;
  cancellationRate: number;
  completionRate: number;
  totalDoctors: number;
  activeDoctors: number;
  totalPatients: number;
  totalRevenue: number;
  revenueBySpecialty: SpecialtyRevenue[];
  appointmentsLast30Days: DailyAppointmentCount[];
  topRatedDoctors: TopRatedDoctor[];
}
