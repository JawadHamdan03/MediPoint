import { createBrowserRouter, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./ProtectedRoute";
import { AppShell } from "../components/layout/AppShell";
import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/admin/DashboardPage";
import AddDoctorPage from "../pages/admin/AddDoctorPage";
import UpdateDoctorPage from "../pages/admin/UpdateDoctorPage";
import RemoveDoctorPage from "../pages/admin/RemoveDoctorPage";
import RegisterPatientPage from "../pages/admin/RegisterPatientPage";
import TodayAppointmentsPage from "../pages/doctor/TodayAppointmentsPage";
import AddAppointmentPage from "../pages/doctor/AddAppointmentPage";
import AddPrescriptionPage from "../pages/doctor/AddPrescriptionPage";
import SearchDoctorsPage from "../pages/patient/SearchDoctorsPage";

const adminNavItems = [
  { to: "/admin", label: "Dashboard" },
  { to: "/admin/doctors/add", label: "Add Doctor" },
  { to: "/admin/doctors/update", label: "Update Doctor" },
  { to: "/admin/doctors/remove", label: "Remove Doctor" },
  { to: "/admin/patients/register", label: "Register Patient" },
];

const doctorNavItems = [
  { to: "/doctor", label: "Today's Appointments" },
  { to: "/doctor/appointments/add", label: "Add Appointment" },
  { to: "/doctor/prescriptions/add", label: "Add Prescription" },
];

const patientNavItems = [{ to: "/patient", label: "Search Doctors" }];

export const router = createBrowserRouter([
  { path: "/", element: <Navigate to="/login" replace /> },
  { path: "/login", element: <LoginPage /> },
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
        children: [{ index: true, element: <SearchDoctorsPage /> }],
      },
    ],
  },
]);
