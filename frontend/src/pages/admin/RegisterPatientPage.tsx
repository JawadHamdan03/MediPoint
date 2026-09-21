import { useState, type FormEvent } from "react";
import { registerPatient } from "../../api/admin";
import type { PatientDto } from "../../types/admin";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

const initial: PatientDto = {
  firstName: "",
  lastName: "",
  password: "",
  email: "",
  phoneNumber: "",
  dateOfBirth: "",
  gender: "Male",
  bloodType: "",
  address: "",
  emergencyContactName: "",
  emergencyContactPhone: "",
};

export default function RegisterPatientPage() {
  const [form, setForm] = useState<PatientDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof PatientDto>(key: K, value: PatientDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSuccess(null);
    setSubmitting(true);
    try {
      const res = await registerPatient(form);
      setSuccess(`Patient ${res.firstName} ${res.lastName} registered.`);
      setForm(initial);
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card className="mx-auto max-w-2xl">
      <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Register Patient</h2>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Input label="First name" required maxLength={50} value={form.firstName} onChange={(e) => update("firstName", e.target.value)} />
        <Input label="Last name" required maxLength={50} value={form.lastName} onChange={(e) => update("lastName", e.target.value)} />
        <Input label="Email" type="email" required maxLength={100} value={form.email} onChange={(e) => update("email", e.target.value)} />
        <Input
          label="Password"
          type="password"
          required
          minLength={8}
          value={form.password}
          onChange={(e) => update("password", e.target.value)}
        />
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
        <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as PatientDto["gender"])}>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </Select>
        <Input label="Blood type" maxLength={10} value={form.bloodType} onChange={(e) => update("bloodType", e.target.value)} />
        <div className="sm:col-span-2">
          <Input label="Address" maxLength={200} value={form.address} onChange={(e) => update("address", e.target.value)} />
        </div>
        <Input
          label="Emergency contact name"
          maxLength={100}
          value={form.emergencyContactName}
          onChange={(e) => update("emergencyContactName", e.target.value)}
        />
        <Input
          label="Emergency contact phone"
          maxLength={20}
          value={form.emergencyContactPhone}
          onChange={(e) => update("emergencyContactPhone", e.target.value)}
        />

        {errors.length > 0 && (
          <div className="sm:col-span-2">
            <ErrorList errors={errors} />
          </div>
        )}
        {success && <p className="sm:col-span-2 text-sm text-green-600">{success}</p>}

        <div className="sm:col-span-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Registering…" : "Register patient"}
          </Button>
        </div>
      </form>
    </Card>
  );
}
