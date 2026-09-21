import type { AppointmentStatus } from "../../types/common";

const statusClasses: Record<AppointmentStatus, string> = {
  Pending: "bg-yellow-100 text-yellow-800",
  Confirmed: "bg-blue-100 text-blue-800",
  Completed: "bg-green-100 text-green-800",
  Cancelled: "bg-gray-200 text-gray-600",
};

export function Badge({ status }: { status: AppointmentStatus }) {
  return (
    <span className={`inline-block rounded-full px-2 py-0.5 text-xs font-medium ${statusClasses[status]}`}>
      {status}
    </span>
  );
}
