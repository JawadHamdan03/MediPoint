import { useState, type FormEvent } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { addPrescription } from "../../api/doctor";
import type { PrescriptionRequest } from "../../types/doctor";
import { Input } from "../../components/ui/Input";
import { Textarea } from "../../components/ui/Textarea";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { PillIcon, RecordsIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

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
  const toast = useToast();
  const [form, setForm] = useState<PrescriptionRequest>(initial(params.get("appointmentId") ?? ""));
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof PrescriptionRequest>(key: K, value: PrescriptionRequest[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      await addPrescription(form);
      toast.notify("Prescription saved.");
      navigate("/doctor", { replace: true });
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <PageHeader icon={PillIcon} title="Add Prescription" description="Medicine and lab result sections are each optional." />
      <Card>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <Input
            label="Appointment id"
            required
            value={form.appointmentId}
            error={fields.appointmentid}
            onChange={(e) => update("appointmentId", e.target.value)}
          />
        </div>
        <div className="sm:col-span-2">
          <Textarea
            label="Notes"
            maxLength={2000}
            rows={3}
            value={form.notes}
            error={fields.notes}
            onChange={(e) => update("notes", e.target.value)}
          />
        </div>

        <p className="sm:col-span-2 -mb-2 flex items-center gap-1.5 text-sm font-semibold text-(--text-h)">
          <PillIcon className="h-4 w-4 text-(--accent)" /> Medicine (optional)
        </p>
        <Input label="Medicine name" value={form.medicineName} error={fields.medicinename} onChange={(e) => update("medicineName", e.target.value)} />
        <Input label="Dosage" value={form.dosage} error={fields.dosage} onChange={(e) => update("dosage", e.target.value)} />
        <Input label="Frequency" value={form.frequency} onChange={(e) => update("frequency", e.target.value)} />
        <Input
          label="Duration (days)"
          type="number"
          min={0}
          value={form.durationDays}
          error={fields.durationdays}
          onChange={(e) => update("durationDays", Number(e.target.value))}
        />
        <div className="sm:col-span-2">
          <Input label="Instructions" value={form.instructions} onChange={(e) => update("instructions", e.target.value)} />
        </div>

        <p className="sm:col-span-2 -mb-2 flex items-center gap-1.5 text-sm font-semibold text-(--text-h)">
          <RecordsIcon className="h-4 w-4 text-(--accent)" /> Lab result (optional)
        </p>
        <Input label="Test name" value={form.testName} error={fields.testname} onChange={(e) => update("testName", e.target.value)} />
        <Input label="Result" value={form.result} error={fields.result} onChange={(e) => update("result", e.target.value)} />
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
    </div>
  );
}
