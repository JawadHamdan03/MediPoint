import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { completeAppointment, getTodaysAppointments } from "../../api/doctor";
import type { AppointmentResponse } from "../../types/doctor";
import { Table } from "../../components/ui/Table";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Spinner } from "../../components/ui/Spinner";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

function CompleteAction({ appointment, onDone }: { appointment: AppointmentResponse; onDone: () => void }) {
  const [open, setOpen] = useState(false);
  const [notes, setNotes] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  if (appointment.status === "Completed" || appointment.status === "Cancelled") {
    return null;
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
      onDone();
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex flex-col gap-2">
      <textarea
        placeholder="Notes (optional)"
        value={notes}
        maxLength={1000}
        onChange={(e) => setNotes(e.target.value)}
        className="w-48 rounded-md border border-(--border) px-2 py-1 text-xs text-(--text-h) outline-none focus:border-(--accent)"
        rows={2}
      />
      {errors.length > 0 && <ErrorList errors={errors} />}
      <div className="flex gap-2">
        <Button onClick={submit} disabled={submitting}>
          {submitting ? "Saving…" : "Confirm"}
        </Button>
        <Button variant="secondary" onClick={() => setOpen(false)}>
          Cancel
        </Button>
      </div>
    </div>
  );
}

export default function TodayAppointmentsPage() {
  const [appointments, setAppointments] = useState<AppointmentResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  async function load() {
    setErrors([]);
    try {
      setAppointments(await getTodaysAppointments());
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  useEffect(() => {
    load();
  }, []);

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold text-(--text-h)">Today's Appointments</h2>
        <Link to="/doctor/appointments/add">
          <Button>Add appointment</Button>
        </Link>
      </div>

      {errors.length > 0 && <ErrorList errors={errors} />}

      {appointments === null ? (
        <Spinner />
      ) : (
        <Table
          rows={appointments}
          rowKey={(a) => a.id}
          emptyMessage="No appointments today."
          columns={[
            { header: "Time", render: (a) => new Date(a.appointmentDate).toLocaleTimeString() },
            { header: "Duration (min)", render: (a) => a.duration },
            { header: "Patient", render: (a) => a.patientId },
            { header: "Status", render: (a) => <Badge status={a.status} /> },
            { header: "Reason", render: (a) => a.reason ?? "—" },
            {
              header: "Actions",
              render: (a) => (
                <div className="flex flex-col gap-2">
                  <CompleteAction appointment={a} onDone={load} />
                  <Link to={`/doctor/prescriptions/add?appointmentId=${a.id}`} className="text-xs text-(--accent) underline">
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
