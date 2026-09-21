import { useState, type FormEvent } from "react";
import { addDoctor } from "../../api/admin";
import type { DoctorDto } from "../../types/admin";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Textarea } from "../../components/ui/Textarea";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { UserPlusIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

const initial: DoctorDto = {
  firstName: "",
  lastName: "",
  password: "",
  email: "",
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

export default function AddDoctorPage() {
  const toast = useToast();
  const [form, setForm] = useState<DoctorDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof DoctorDto>(key: K, value: DoctorDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      const res = await addDoctor(form);
      toast.notify(`Doctor ${res.firstName} ${res.lastName} added.`);
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
      <PageHeader icon={UserPlusIcon} title="Add Doctor" description="Onboard a new doctor account." />
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
        <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as DoctorDto["gender"])}>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </Select>
        <Input
          label="Specialty"
          required
          maxLength={100}
          value={form.specialty}
          error={fields.specialty}
          onChange={(e) => update("specialty", e.target.value)}
        />
        <Input
          label="License number"
          required
          maxLength={50}
          value={form.licenseNumber}
          error={fields.licensenumber}
          onChange={(e) => update("licenseNumber", e.target.value)}
        />
        <Input
          label="Years of experience"
          type="number"
          min={0}
          max={70}
          value={form.yearsOfExperience}
          error={fields.yearsofexperience}
          onChange={(e) => update("yearsOfExperience", Number(e.target.value))}
        />
        <Input
          label="Consultation fee"
          type="number"
          min={0}
          step="0.01"
          value={form.consultationFee}
          error={fields.consultationfee}
          onChange={(e) => update("consultationFee", Number(e.target.value))}
        />
        <div className="sm:col-span-2">
          <Textarea
            label="Biography"
            maxLength={1000}
            rows={3}
            value={form.biography}
            error={fields.biography}
            onChange={(e) => update("biography", e.target.value)}
          />
        </div>

        {errors.length > 0 && (
          <div className="sm:col-span-2">
            <ErrorList errors={errors} />
          </div>
        )}

        <div className="sm:col-span-2">
          <Button type="submit" disabled={submitting}>
            {submitting ? "Adding…" : "Add doctor"}
          </Button>
        </div>
      </form>
      </Card>
    </div>
  );
}
