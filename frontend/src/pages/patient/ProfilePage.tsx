import { useState, type ChangeEvent, type FormEvent } from "react";
import { updateDetails } from "../../api/patient";
import { uploadProfileImage } from "../../api/users";
import type { UpdatePatientDto } from "../../types/patient";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

const initial: UpdatePatientDto = {
  firstName: "",
  lastName: "",
  phoneNumber: "",
  dateOfBirth: "",
  gender: "Male",
  bloodType: "",
  address: "",
  emergencyContactName: "",
  emergencyContactPhone: "",
};

export default function ProfilePage() {
  const [form, setForm] = useState<UpdatePatientDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const [imageErrors, setImageErrors] = useState<string[]>([]);
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  function update<K extends keyof UpdatePatientDto>(key: K, value: UpdatePatientDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSuccess(null);
    setSubmitting(true);
    try {
      await updateDetails(form);
      setSuccess("Details updated.");
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  async function handleImageChange(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setImageErrors([]);
    setUploading(true);
    try {
      const res = await uploadProfileImage(file);
      setImageUrl(res.imageUrl);
    } catch (err) {
      setImageErrors(formatError(err));
    } finally {
      setUploading(false);
    }
  }

  return (
    <div className="mx-auto flex max-w-2xl flex-col gap-6">
      <Card>
        <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Profile Photo</h2>
        {imageUrl && <img src={imageUrl} alt="Profile" className="mb-3 h-24 w-24 rounded-full object-cover" />}
        <input type="file" accept="image/*" onChange={handleImageChange} disabled={uploading} />
        {uploading && <p className="mt-2 text-sm text-(--text)">Uploading…</p>}
        {imageErrors.length > 0 && (
          <div className="mt-2">
            <ErrorList errors={imageErrors} />
          </div>
        )}
      </Card>

      <Card>
        <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Update Details</h2>
        <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
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
          <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as UpdatePatientDto["gender"])}>
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
              {submitting ? "Saving…" : "Save details"}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
