import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { addAppointment } from "../../api/doctor";
import { useAuth } from "../../context/AuthContext";
import { useToast } from "../../context/ToastContext";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { CalendarPlusIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";

export default function AddAppointmentPage() {
  const auth = useAuth();
  const toast = useToast();
  const navigate = useNavigate();
  const [appointmentDate, setAppointmentDate] = useState("");
  const [duration, setDuration] = useState(30);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      await addAppointment({
        appointmentDate: new Date(appointmentDate).toISOString(),
        duration,
        doctorId: auth.userId!,
      });
      toast.notify("Appointment slot added.");
      navigate("/doctor", { replace: true });
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-md">
      <PageHeader icon={CalendarPlusIcon} title="Add Appointment Slot" description="Create an open slot patients can book." />
      <Card>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label="Date & time"
          type="datetime-local"
          required
          value={appointmentDate}
          error={fields.appointmentdate}
          onChange={(e) => setAppointmentDate(e.target.value)}
        />
        <Input
          label="Duration (minutes)"
          type="number"
          required
          min={1}
          value={duration}
          error={fields.duration}
          onChange={(e) => setDuration(Number(e.target.value))}
        />
        {errors.length > 0 && <ErrorList errors={errors} />}
        <Button type="submit" disabled={submitting} className="w-full">
          {submitting ? "Adding…" : "Add appointment"}
        </Button>
      </form>
      </Card>
    </div>
  );
}
