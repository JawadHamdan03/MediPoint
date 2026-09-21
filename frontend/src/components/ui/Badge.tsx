import type { AppointmentStatus } from "../../types/common";

const statusClasses: Record<AppointmentStatus, string> = {
  Pending: "bg-amber-100 text-amber-800 dark:bg-amber-400/15 dark:text-amber-300",
  Confirmed: "bg-blue-100 text-blue-800 dark:bg-blue-400/15 dark:text-blue-300",
  Completed: "bg-green-100 text-green-800 dark:bg-green-400/15 dark:text-green-300",
  Cancelled: "bg-gray-200 text-gray-600 dark:bg-gray-400/15 dark:text-gray-400",
};

export function Badge({ status }: { status: AppointmentStatus }) {
  return (
    <span className={`inline-block whitespace-nowrap rounded-full px-2 py-0.5 text-xs font-medium ${statusClasses[status]}`}>
      {status}
    </span>
  );
}
