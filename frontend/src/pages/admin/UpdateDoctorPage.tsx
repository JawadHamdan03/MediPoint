import { useState, type FormEvent } from "react";
import { updateDoctor } from "../../api/admin";
import type { UpdateDoctorDto } from "../../types/admin";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

const initial: UpdateDoctorDto = {
  firstName: "",
  lastName: "",
  phoneNumber: "",
  dateOfBirth: "",
  gender: "Male",
  specialty: "",
  licenseNumber: "",
  yearsOfExperience: 0,
  consultationFee: 0,
  biography: "",
  isAvailable: true,
};

export default function UpdateDoctorPage() {
  const [doctorId, setDoctorId] = useState("");
  const [form, setForm] = useState<UpdateDoctorDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof UpdateDoctorDto>(key: K, value: UpdateDoctorDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSuccess(null);
    setSubmitting(true);
    try {
      const res = await updateDoctor(doctorId, form);
      setSuccess(`Doctor ${res.firstName} ${res.lastName} updated.`);
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card className="mx-auto max-w-2xl">
      <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Update Doctor</h2>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <Input label="Doctor id" required value={doctorId} onChange={(e) => setDoctorId(e.target.value)} />
        </div>
        <Input label="First name" required maxLength={50} value={form.firstName} onChange={(e) => update("firstName", e.target.value)} />
        <Input label="Last name" required maxLength={50} value={form.lastName} onChange={(e) => update("lastName", e.target.value)} />
        <Input
          label="Phone number"
          required
          placeholder="+15551234567"
          pattern="^\+?[1-9]\d{1,14}$"
          value={form.phoneNumber}
          onChange={(e) => update("phoneNumber", e.target.value)}
        />
        <Input
          label="Date of birth"
          type="date"
          required
          value={form.dateOfBirth}
          onChange={(e) => update("dateOfBirth", e.target.value)}
        />
        <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as UpdateDoctorDto["gender"])}>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </Select>
        <Input label="Specialty" required maxLength={100} value={form.specialty} onChange={(e) => update("specialty", e.target.value)} />
        <Input
          label="License number"
          required
          maxLength={50}
          value={form.licenseNumber}
          onChange={(e) => update("licenseNumber", e.target.value)}
        />
        <Input
          label="Years of experience"
          type="number"
          min={0}
          max={70}
          value={form.yearsOfExperience}
          onChange={(e) => update("yearsOfExperience", Number(e.target.value))}
        />
        <Input
          label="Consultation fee"
          type="number"
          min={0}
          step="0.01"
          value={form.consultationFee}
          onChange={(e) => update("consultationFee", Number(e.target.value))}
        />
        <label className="flex items-center gap-2 text-sm text-(--text)">
          <input
            type="checkbox"
            checked={form.isAvailable}
            onChange={(e) => update("isAvailable", e.target.checked)}
          />
          Available
        </label>
        <div className="sm:col-span-2">
          <label className="flex flex-col gap-1 text-sm text-(--text)">
            Biography
            <textarea
              maxLength={1000}
              rows={3}
              value={form.biography}
              onChange={(e) => update("biography", e.target.value)}
              className="rounded-md border border-(--border) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent)"
            />
          </label>
        </div>

        {errors.length > 0 && (
          <div className="sm:col-span-2">
            <ErrorList errors={errors} />
          </div>
        )}
        {success && <p className="sm:col-span-2 text-sm text-green-600">{success}</p>}

        <div className="sm:col-span-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Updating…" : "Update doctor"}
          </Button>
        </div>
      </form>
    </Card>
  );
}
