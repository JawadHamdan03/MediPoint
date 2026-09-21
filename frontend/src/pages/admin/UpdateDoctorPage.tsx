import { useState, type FormEvent } from "react";
import { updateDoctor } from "../../api/admin";
import type { UpdateDoctorDto } from "../../types/admin";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Textarea } from "../../components/ui/Textarea";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { UserEditIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

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
  const toast = useToast();
  const [doctorId, setDoctorId] = useState("");
  const [form, setForm] = useState<UpdateDoctorDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function update<K extends keyof UpdateDoctorDto>(key: K, value: UpdateDoctorDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      const res = await updateDoctor(doctorId, form);
      toast.notify(`Doctor ${res.firstName} ${res.lastName} updated.`);
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <PageHeader icon={UserEditIcon} title="Update Doctor" description="Edit an existing doctor's profile by id." />
      <Card>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <Input label="Doctor id" required value={doctorId} error={fields.doctorid} onChange={(e) => setDoctorId(e.target.value)} />
        </div>
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
        <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as UpdateDoctorDto["gender"])}>
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
        <label className="flex items-center gap-2 self-end pb-2 text-sm text-(--text)">
          <input type="checkbox" checked={form.isAvailable} onChange={(e) => update("isAvailable", e.target.checked)} />
          Available
        </label>
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
            {submitting ? "Updating…" : "Update doctor"}
          </Button>
        </div>
      </form>
      </Card>
    </div>
  );
}
