import { createBrowserRouter, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./ProtectedRoute";
import { AppShell } from "../components/layout/AppShell";
import LoginPage from "../pages/auth/LoginPage";
import SignUpPage from "../pages/auth/SignUpPage";
import DashboardPage from "../pages/admin/DashboardPage";
import AddDoctorPage from "../pages/admin/AddDoctorPage";
import UpdateDoctorPage from "../pages/admin/UpdateDoctorPage";
import RemoveDoctorPage from "../pages/admin/RemoveDoctorPage";
import RegisterPatientPage from "../pages/admin/RegisterPatientPage";
import TodayAppointmentsPage from "../pages/doctor/TodayAppointmentsPage";
import AddAppointmentPage from "../pages/doctor/AddAppointmentPage";
import AddPrescriptionPage from "../pages/doctor/AddPrescriptionPage";
import SearchDoctorsPage from "../pages/patient/SearchDoctorsPage";
import MedicalRecordsPage from "../pages/patient/MedicalRecordsPage";
import ChatPage from "../pages/patient/ChatPage";
import ProfilePage from "../pages/patient/ProfilePage";
import {
  CalendarIcon,
  CalendarPlusIcon,
  ChatIcon,
  DashboardIcon,
  PillIcon,
  ProfileIcon,
  RecordsIcon,
  SearchIcon,
  UserEditIcon,
  UserMinusIcon,
  UserPlusIcon,
} from "../components/ui/icons";

const adminNavItems = [
  { to: "/admin", label: "Dashboard", icon: DashboardIcon },
  { to: "/admin/doctors/add", label: "Add Doctor", icon: UserPlusIcon },
  { to: "/admin/doctors/update", label: "Update Doctor", icon: UserEditIcon },
  { to: "/admin/doctors/remove", label: "Remove Doctor", icon: UserMinusIcon },
  { to: "/admin/patients/register", label: "Register Patient", icon: UserPlusIcon },
];

const doctorNavItems = [
  { to: "/doctor", label: "Today's Appointments", icon: CalendarIcon },
  { to: "/doctor/appointments/add", label: "Add Appointment", icon: CalendarPlusIcon },
  { to: "/doctor/prescriptions/add", label: "Add Prescription", icon: PillIcon },
];

const patientNavItems = [
  { to: "/patient", label: "Search Doctors", icon: SearchIcon },
  { to: "/patient/medical-records", label: "Medical Records", icon: RecordsIcon },
  { to: "/patient/chat", label: "Chat", icon: ChatIcon },
  { to: "/patient/profile", label: "Profile", icon: ProfileIcon },
];

export const router = createBrowserRouter([
  { path: "/", element: <Navigate to="/login" replace /> },
  { path: "/login", element: <LoginPage /> },
  { path: "/signup", element: <SignUpPage /> },
  {
    path: "/admin",
    element: <ProtectedRoute role="Admin" />,
    children: [
      {
        element: <AppShell navItems={adminNavItems} title="Admin" />,
        children: [
          { index: true, element: <DashboardPage /> },
          { path: "doctors/add", element: <AddDoctorPage /> },
          { path: "doctors/update", element: <UpdateDoctorPage /> },
          { path: "doctors/remove", element: <RemoveDoctorPage /> },
          { path: "patients/register", element: <RegisterPatientPage /> },
        ],
      },
    ],
  },
  {
    path: "/doctor",
    element: <ProtectedRoute role="Doctor" />,
    children: [
      {
        element: <AppShell navItems={doctorNavItems} title="Doctor" />,
        children: [
          { index: true, element: <TodayAppointmentsPage /> },
          { path: "appointments/add", element: <AddAppointmentPage /> },
          { path: "prescriptions/add", element: <AddPrescriptionPage /> },
        ],
      },
    ],
  },
  {
    path: "/patient",
    element: <ProtectedRoute role="Patient" />,
    children: [
      {
        element: <AppShell navItems={patientNavItems} title="Patient" />,
        children: [
          { index: true, element: <SearchDoctorsPage /> },
          { path: "medical-records", element: <MedicalRecordsPage /> },
          { path: "chat", element: <ChatPage /> },
          { path: "profile", element: <ProfilePage /> },
        ],
      },
    ],
  },
]);
