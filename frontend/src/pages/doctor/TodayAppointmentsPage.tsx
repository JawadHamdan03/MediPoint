import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { completeAppointment, getTodaysAppointments } from "../../api/doctor";
import type { AppointmentResponse } from "../../types/doctor";
import { Table } from "../../components/ui/Table";
import { TableSkeleton } from "../../components/ui/Skeleton";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Avatar } from "../../components/ui/Avatar";
import { PageHeader } from "../../components/ui/PageHeader";
import { CalendarIcon, CalendarPlusIcon, PillIcon, RefreshIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

function CompleteAction({ appointment, onDone }: { appointment: AppointmentResponse; onDone: () => void }) {
  const toast = useToast();
  const [open, setOpen] = useState(false);
  const [notes, setNotes] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  if (appointment.status === "Completed" || appointment.status === "Cancelled") {
    return <span className="text-xs text-(--text)">—</span>;
  }

  if (!open) {
    return (
      <Button variant="secondary" onClick={() => setOpen(true)}>
        Complete
      </Button>
    );
  }

  async function submit() {
    setSubmitting(true);
    setErrors([]);
    try {
      await completeAppointment(appointment.id, { notes: notes || null });
      toast.notify("Appointment marked complete.");
      onDone();
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex w-52 flex-col gap-2 rounded-(--radius) border border-(--border) bg-(--bg) p-2" style={{ animation: "fade-in 0.1s ease-out" }}>
      <textarea
        placeholder="Notes (optional)"
        value={notes}
        maxLength={1000}
        onChange={(e) => setNotes(e.target.value)}
        className="w-full rounded-(--radius) border border-(--border) bg-(--surface) px-2 py-1 text-xs text-(--text-h) outline-none focus:border-(--accent)"
        rows={2}
        autoFocus
      />
      {errors.length > 0 && <ErrorList errors={errors} />}
      <div className="flex gap-2">
        <Button onClick={submit} disabled={submitting}>
          {submitting ? "Saving…" : "Confirm"}
        </Button>
        <Button variant="secondary" onClick={() => setOpen(false)} disabled={submitting}>
          Cancel
        </Button>
      </div>
    </div>
  );
}

export default function TodayAppointmentsPage() {
  const [appointments, setAppointments] = useState<AppointmentResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [refreshing, setRefreshing] = useState(false);

  async function load(showSpinner = false) {
    setErrors([]);
    if (showSpinner) setRefreshing(true);
    try {
      setAppointments(await getTodaysAppointments());
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      if (showSpinner) setRefreshing(false);
    }
  }

  useEffect(() => {
    // Standard fetch-on-mount: load() resets error state before the request.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, []);

  const pendingCount = appointments?.filter((a) => a.status === "Pending" || a.status === "Confirmed").length ?? 0;

  return (
    <div className="flex flex-col gap-4">
      <PageHeader
        icon={CalendarIcon}
        title="Today's Appointments"
        description={appointments ? `${pendingCount} upcoming · ${appointments.length} total` : "Loading your schedule…"}
        action={
          <div className="flex gap-2">
            <Button variant="secondary" onClick={() => load(true)} disabled={refreshing}>
              <RefreshIcon className={`h-4 w-4 ${refreshing ? "animate-spin" : ""}`} />
              Refresh
            </Button>
            <Link to="/doctor/appointments/add">
              <Button>
                <CalendarPlusIcon className="h-4 w-4" />
                Add appointment
              </Button>
            </Link>
          </div>
        }
      />

      {errors.length > 0 && <ErrorList errors={errors} />}

      {appointments === null ? (
        <TableSkeleton rows={5} cols={5} />
      ) : (
        <Table
          rows={appointments}
          rowKey={(a) => a.id}
          emptyMessage="No appointments today."
          columns={[
            { header: "Time", render: (a) => new Date(a.appointmentDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }) },
            { header: "Duration (min)", render: (a) => `${a.duration} min` },
            {
              header: "Patient",
              render: (a) => (
                <div className="flex items-center gap-2">
                  <Avatar name={a.patientId} size="sm" />
                  <span className="font-mono text-xs text-(--text)">{a.patientId.slice(0, 8)}</span>
                </div>
              ),
            },
            { header: "Status", render: (a) => <Badge status={a.status} /> },
            { header: "Reason", render: (a) => a.reason ?? "—" },
            {
              header: "Actions",
              render: (a) => (
                <div className="flex flex-col items-start gap-2">
                  <CompleteAction appointment={a} onDone={() => load()} />
                  <Link
                    to={`/doctor/prescriptions/add?appointmentId=${a.id}`}
                    className="flex items-center gap-1 text-xs font-medium text-(--accent) hover:underline"
                  >
                    <PillIcon className="h-3.5 w-3.5" />
                    Add prescription
                  </Link>
                </div>
              ),
            },
          ]}
        />
      )}
    </div>
  );
}
