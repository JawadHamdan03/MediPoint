import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { addAppointment } from "../../api/doctor";
import { useAuth } from "../../context/AuthContext";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

export default function AddAppointmentPage() {
  const auth = useAuth();
  const navigate = useNavigate();
  const [appointmentDate, setAppointmentDate] = useState("");
  const [duration, setDuration] = useState(30);
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSubmitting(true);
    try {
      await addAppointment({
        appointmentDate: new Date(appointmentDate).toISOString(),
        duration,
        doctorId: auth.userId!,
      });
      navigate("/doctor", { replace: true });
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card className="mx-auto max-w-md">
      <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Add Appointment Slot</h2>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label="Date & time"
          type="datetime-local"
          required
          value={appointmentDate}
          onChange={(e) => setAppointmentDate(e.target.value)}
        />
        <Input
          label="Duration (minutes)"
          type="number"
          required
          min={1}
          value={duration}
          onChange={(e) => setDuration(Number(e.target.value))}
        />
        {errors.length > 0 && <ErrorList errors={errors} />}
        <Button type="submit" disabled={submitting}>
          {submitting ? "Adding…" : "Add appointment"}
        </Button>
      </form>
    </Card>
  );
}
