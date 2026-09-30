import { apiFetch } from "./client";
import type { NotificationResponse } from "../types/common";

export interface UploadProfileImageResult {
  imageUrl: string;
}

export function uploadProfileImage(file: File): Promise<UploadProfileImageResult> {
  const form = new FormData();
  form.append("image", file);
  return apiFetch<UploadProfileImageResult>("/users/profile-image", { method: "POST", body: form, isForm: true });
}

export function getNotifications(): Promise<NotificationResponse[]> {
  return apiFetch<NotificationResponse[]>("/users/notifications");
}

export function markNotificationRead(notificationId: string): Promise<void> {
  return apiFetch<void>(`/users/notifications/${notificationId}/read`, { method: "POST" });
}

export function markAllNotificationsRead(): Promise<void> {
  return apiFetch<void>("/users/notifications/read-all", { method: "POST" });
}
