import { useState, type FormEvent } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { addPrescription } from "../../api/doctor";
import type { PrescriptionRequest } from "../../types/doctor";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

function initial(appointmentId: string): PrescriptionRequest {
  return {
    notes: "",
    appointmentId,
    medicineName: "",
    dosage: "",
    frequency: "",
    durationDays: 0,
    instructions: "",
    testName: "",
    result: "",
    unit: "",
    referenceRange: "",
  };
}

export default function AddPrescriptionPage() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const [form, setForm] = useState<PrescriptionRequest>(initial(params.get("appointmentId") ?? ""));
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof PrescriptionRequest>(key: K, value: PrescriptionRequest[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSubmitting(true);
    try {
      await addPrescription(form);
      navigate("/doctor", { replace: true });
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card className="mx-auto max-w-2xl">
      <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Add Prescription</h2>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <Input
            label="Appointment id"
            required
            value={form.appointmentId}
            onChange={(e) => update("appointmentId", e.target.value)}
          />
        </div>
        <div className="sm:col-span-2">
          <label className="flex flex-col gap-1 text-sm text-(--text)">
            Notes
            <textarea
              maxLength={2000}
              rows={3}
              value={form.notes}
              onChange={(e) => update("notes", e.target.value)}
              className="rounded-md border border-(--border) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent)"
            />
          </label>
        </div>

        <p className="sm:col-span-2 text-sm font-medium text-(--text-h)">Medicine (optional)</p>
        <Input label="Medicine name" value={form.medicineName} onChange={(e) => update("medicineName", e.target.value)} />
        <Input label="Dosage" value={form.dosage} onChange={(e) => update("dosage", e.target.value)} />
        <Input label="Frequency" value={form.frequency} onChange={(e) => update("frequency", e.target.value)} />
        <Input
          label="Duration (days)"
          type="number"
          min={0}
          value={form.durationDays}
          onChange={(e) => update("durationDays", Number(e.target.value))}
        />
        <div className="sm:col-span-2">
          <Input label="Instructions" value={form.instructions} onChange={(e) => update("instructions", e.target.value)} />
        </div>

        <p className="sm:col-span-2 text-sm font-medium text-(--text-h)">Lab result (optional)</p>
        <Input label="Test name" value={form.testName} onChange={(e) => update("testName", e.target.value)} />
        <Input label="Result" value={form.result} onChange={(e) => update("result", e.target.value)} />
        <Input label="Unit" value={form.unit} onChange={(e) => update("unit", e.target.value)} />
        <Input label="Reference range" value={form.referenceRange} onChange={(e) => update("referenceRange", e.target.value)} />

        {errors.length > 0 && (
          <div className="sm:col-span-2">
            <ErrorList errors={errors} />
          </div>
        )}

        <div className="sm:col-span-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Saving…" : "Save prescription"}
          </Button>
        </div>
      </form>
    </Card>
  );
}
