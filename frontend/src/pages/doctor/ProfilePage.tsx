import { useEffect, useState, type ChangeEvent } from "react";
import { uploadProfileImage } from "../../api/users";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { CameraIcon, ProfileIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";
import { useAuth } from "../../context/AuthContext";

export default function DoctorProfilePage() {
  const toast = useToast();
  const auth = useAuth();

  const [imageErrors, setImageErrors] = useState<string[]>([]);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl);
    };
  }, [previewUrl]);

  async function handleImageChange(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setImageErrors([]);
    setPreviewUrl(URL.createObjectURL(file));
    setUploading(true);
    try {
      const res = await uploadProfileImage(file);
      auth.setProfileImage(res.imageUrl);
      toast.notify("Profile photo updated.");
    } catch (err) {
      setImageErrors(formatError(err));
    } finally {
      setUploading(false);
    }
  }

  const displayedImage = previewUrl ?? auth.imageUrl;

  return (
    <div className="mx-auto flex max-w-2xl flex-col gap-6">
      <PageHeader icon={ProfileIcon} title="Profile" description="Manage your profile photo." />

      <Card>
        <h3 className="mb-1 text-sm font-semibold text-(--text-h)">Profile Photo</h3>
        <p className="mb-4 text-sm text-(--text)">JPG or PNG, shown to patients and staff.</p>
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
    </div>
  );
}
