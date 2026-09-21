import { useState, type FormEvent } from "react";
import { removeDoctor } from "../../api/admin";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

export default function RemoveDoctorPage() {
  const [doctorId, setDoctorId] = useState("");
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSuccess(null);
    setSubmitting(true);
    try {
      const res = await removeDoctor(doctorId);
      setSuccess(`Doctor ${res.firstName} ${res.lastName} deactivated.`);
      setDoctorId("");
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card className="mx-auto max-w-md">
      <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Remove Doctor</h2>
      <p className="mb-4 text-sm text-(--text)">This deactivates the doctor (soft delete) rather than deleting the record.</p>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input label="Doctor id" required value={doctorId} onChange={(e) => setDoctorId(e.target.value)} />
        {errors.length > 0 && <ErrorList errors={errors} />}
        {success && <p className="text-sm text-green-600">{success}</p>}
        <Button type="submit" variant="danger" disabled={submitting}>
          {submitting ? "Removing…" : "Remove doctor"}
        </Button>
      </form>
    </Card>
  );
}
