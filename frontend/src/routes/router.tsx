import { createBrowserRouter, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./ProtectedRoute";
import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/admin/DashboardPage";
import TodayAppointmentsPage from "../pages/doctor/TodayAppointmentsPage";
import SearchDoctorsPage from "../pages/patient/SearchDoctorsPage";

export const router = createBrowserRouter([
  { path: "/", element: <Navigate to="/login" replace /> },
  { path: "/login", element: <LoginPage /> },
  {
    path: "/admin",
    element: <ProtectedRoute role="Admin" />,
    children: [{ index: true, element: <DashboardPage /> }],
  },
  {
    path: "/doctor",
    element: <ProtectedRoute role="Doctor" />,
    children: [{ index: true, element: <TodayAppointmentsPage /> }],
  },
  {
    path: "/patient",
    element: <ProtectedRoute role="Patient" />,
    children: [{ index: true, element: <SearchDoctorsPage /> }],
  },
]);
