import { apiFetch } from "./client";

export interface UploadProfileImageResult {
  imageUrl: string;
}

export function uploadProfileImage(file: File): Promise<UploadProfileImageResult> {
  const form = new FormData();
  form.append("image", file);
  return apiFetch<UploadProfileImageResult>("/users/profile-image", { method: "POST", body: form, isForm: true });
}
