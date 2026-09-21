import { useEffect, useState, type ChangeEvent, type FormEvent } from "react";
import { updateDetails } from "../../api/patient";
import { uploadProfileImage } from "../../api/users";
import type { UpdatePatientDto } from "../../types/patient";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { CameraIcon, ProfileIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

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
  const toast = useToast();
  const [form, setForm] = useState<UpdatePatientDto>(initial);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  const [imageErrors, setImageErrors] = useState<string[]>([]);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl);
    };
  }, [previewUrl]);

  function update<K extends keyof UpdatePatientDto>(key: K, value: UpdatePatientDto[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      await updateDetails(form);
      toast.notify("Details updated.");
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  async function handleImageChange(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setImageErrors([]);
    setPreviewUrl(URL.createObjectURL(file));
    setUploading(true);
    try {
      const res = await uploadProfileImage(file);
      setImageUrl(res.imageUrl);
      toast.notify("Profile photo updated.");
    } catch (err) {
      setImageErrors(formatError(err));
    } finally {
      setUploading(false);
    }
  }

  const displayedImage = previewUrl ?? imageUrl;

  return (
    <div className="mx-auto flex max-w-2xl flex-col gap-6">
      <PageHeader icon={ProfileIcon} title="Profile" description="Manage your photo and personal details." />

      <Card>
        <h3 className="mb-1 text-sm font-semibold text-(--text-h)">Profile Photo</h3>
        <p className="mb-4 text-sm text-(--text)">JPG or PNG, shown to doctors and staff.</p>
        <div className="flex items-center gap-4">
          <div className="relative h-20 w-20 shrink-0 overflow-hidden rounded-full border border-(--border) bg-(--accent-bg)">
            {displayedImage ? (
              <img src={displayedImage} alt="Profile" className="h-full w-full object-cover" />
            ) : (
              <span className="flex h-full w-full items-center justify-center text-(--text)">
                <ProfileIcon className="h-8 w-8" />
              </span>
            )}
            {uploading && (
              <div className="absolute inset-0 flex items-center justify-center bg-black/40">
                <span className="h-5 w-5 animate-spin rounded-full border-2 border-white/40 border-t-white" />
              </div>
            )}
          </div>
          <label className="flex cursor-pointer items-center gap-1.5 rounded-(--radius) border border-(--border) bg-(--surface) px-3.5 py-2 text-sm font-medium text-(--text-h) transition-colors hover:bg-(--accent-bg)">
            <CameraIcon className="h-4 w-4" />
            {displayedImage ? "Change photo" : "Upload photo"}
            <input type="file" accept="image/*" onChange={handleImageChange} disabled={uploading} className="hidden" />
          </label>
        </div>
        {imageErrors.length > 0 && (
          <div className="mt-3">
            <ErrorList errors={imageErrors} />
          </div>
        )}
      </Card>

      <Card>
        <h3 className="mb-1 text-sm font-semibold text-(--text-h)">Update Details</h3>
        <p className="mb-5 text-sm text-(--text)">Keep your contact and emergency info current.</p>
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
          <Select label="Gender" value={form.gender} onChange={(e) => update("gender", e.target.value as UpdatePatientDto["gender"])}>
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
              {submitting ? "Saving…" : "Save details"}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
