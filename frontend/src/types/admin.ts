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
