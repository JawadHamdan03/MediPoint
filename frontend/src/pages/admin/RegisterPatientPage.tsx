import { useState, type FormEvent } from "react";
import { registerPatient } from "../../api/admin";
import type { PatientDto } from "../../types/admin";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { UserPlusIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

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
  const toast = useToast();
  const [form, setForm] = useState<PatientDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof PatientDto>(key: K, value: PatientDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      const res = await registerPatient(form);
      toast.notify(`Patient ${res.firstName} ${res.lastName} registered.`);
      setForm(initial);
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <PageHeader icon={UserPlusIcon} title="Register Patient" description="Create a new patient account." />
      <Card>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Input
          label="First name"
          required
          maxLength={50}
          value={form.firstName}
          error={fields.firstname}
          onChange={(e) => update("firstName", e.target.value)}
        />
        <Input
          label="Last name"
          required
          maxLength={50}
          value={form.lastName}
          error={fields.lastname}
          onChange={(e) => update("lastName", e.target.value)}
        />
        <Input
          label="Email"
          type="email"
          required
          maxLength={100}
          value={form.email}
          error={fields.email}
          onChange={(e) => update("email", e.target.value)}
        />
        <Input
          label="Password"
          type="password"
          required
          minLength={8}
          value={form.password}
          error={fields.password}
          onChange={(e) => update("password", e.target.value)}
        />
        <Input
          label="Phone number"
          required
          placeholder="+15551234567"
          pattern="^\+?[1-9]\d{1,14}$"
          value={form.phoneNumber}
          error={fields.phonenumber}
          onChange={(e) => update("phoneNumber", e.target.value)}
        />
        <Input
          label="Date of birth"
          type="date"
          required
          value={form.dateOfBirth}
          error={fields.dateofbirth}
          onChange={(e) => update("dateOfBirth", e.target.value)}
        />
        <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as PatientDto["gender"])}>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </Select>
        <Input
          label="Blood type"
          maxLength={10}
          value={form.bloodType}
          error={fields.bloodtype}
          onChange={(e) => update("bloodType", e.target.value)}
        />
        <div className="sm:col-span-2">
          <Input label="Address" maxLength={200} value={form.address} error={fields.address} onChange={(e) => update("address", e.target.value)} />
        </div>
        <Input
          label="Emergency contact name"
          maxLength={100}
          value={form.emergencyContactName}
          error={fields.emergencycontactname}
          onChange={(e) => update("emergencyContactName", e.target.value)}
        />
        <Input
          label="Emergency contact phone"
          maxLength={20}
          value={form.emergencyContactPhone}
          error={fields.emergencycontactphone}
          onChange={(e) => update("emergencyContactPhone", e.target.value)}
        />

        {errors.length > 0 && (
          <div className="sm:col-span-2">
            <ErrorList errors={errors} />
          </div>
        )}

        <div className="sm:col-span-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Registering…" : "Register patient"}
          </Button>
        </div>
      </form>
      </Card>
    </div>
  );
}
