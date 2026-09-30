import { useEffect, useRef, useState } from "react";
import { getNotifications, markAllNotificationsRead, markNotificationRead } from "../../api/users";
import type { NotificationResponse } from "../../types/common";
import { BellIcon } from "./icons";
import { Spinner } from "./Spinner";

const POLL_INTERVAL_MS = 30_000;

function timeAgo(iso: string): string {
  const diffMs = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diffMs / 60_000);
  if (minutes < 1) return "just now";
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h ago`;
  const days = Math.floor(hours / 24);
  return `${days}d ago`;
}

export function NotificationBell() {
  const [notifications, setNotifications] = useState<NotificationResponse[] | null>(null);
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  async function load() {
    try {
      setNotifications(await getNotifications());
    } catch {
      // Silent: a failed notification fetch shouldn't disrupt the rest of the UI.
    }
  }

  useEffect(() => {
    // Standard fetch-on-mount, then poll: load() only ever calls setState from its own
    // resolved promise, never synchronously during render.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
    const interval = window.setInterval(load, POLL_INTERVAL_MS);
    return () => window.clearInterval(interval);
  }, []);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const unreadCount = notifications?.filter((n) => !n.isRead).length ?? 0;

  async function handleToggle() {
    const next = !open;
    setOpen(next);
    if (next) await load();
  }

  async function handleNotificationClick(notification: NotificationResponse) {
    if (notification.isRead) return;
    setNotifications((list) => list?.map((n) => (n.id === notification.id ? { ...n, isRead: true } : n)) ?? list);
    try {
      await markNotificationRead(notification.id);
    } catch {
      load();
    }
  }

  async function handleMarkAllRead() {
    setLoading(true);
    setNotifications((list) => list?.map((n) => ({ ...n, isRead: true })) ?? list);
    try {
      await markAllNotificationsRead();
    } catch {
      load();
    } finally {
      setLoading(false);
    }
  }

  return (
    <div ref={containerRef} className="relative">
      <button
        type="button"
        onClick={handleToggle}
        aria-label="Notifications"
        className="relative flex h-8 w-8 items-center justify-center rounded-(--radius) text-(--text) transition-colors hover:bg-(--accent-bg) hover:text-(--text-h)"
      >
        <BellIcon className="h-4.5 w-4.5" />
        {unreadCount > 0 && (
          <span className="absolute -top-0.5 -right-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-(--accent) px-1 text-[10px] font-semibold text-white">
            {unreadCount > 9 ? "9+" : unreadCount}
          </span>
        )}
      </button>

      {open && (
        <div
          className="absolute top-full right-0 z-20 mt-2 w-80 max-w-[calc(100vw-2rem)] rounded-(--radius) border border-(--border) bg-(--surface) shadow-(--shadow)"
          style={{ animation: "fade-in 0.1s ease-out" }}
        >
          <div className="flex items-center justify-between border-b border-(--border) px-3 py-2.5">
            <p className="text-sm font-semibold text-(--text-h)">Notifications</p>
            {unreadCount > 0 && (
              <button
                type="button"
                onClick={handleMarkAllRead}
                disabled={loading}
                className="text-xs font-medium text-(--accent) hover:underline disabled:opacity-50"
              >
                Mark all read
              </button>
            )}
          </div>

          <div className="max-h-96 overflow-y-auto">
            {notifications === null ? (
              <div className="flex justify-center py-6">
                <Spinner />
              </div>
            ) : notifications.length === 0 ? (
              <p className="px-3 py-6 text-center text-sm text-(--text)">No notifications yet.</p>
            ) : (
              notifications.map((n) => (
                <button
                  key={n.id}
                  type="button"
                  onClick={() => handleNotificationClick(n)}
                  className={`flex w-full flex-col gap-0.5 border-b border-(--border) px-3 py-2.5 text-left transition-colors last:border-b-0 hover:bg-(--accent-bg) ${
                    n.isRead ? "" : "bg-(--accent-bg)"
                  }`}
                >
                  <div className="flex items-center gap-1.5">
                    {!n.isRead && <span className="h-1.5 w-1.5 shrink-0 rounded-full bg-(--accent)" />}
                    <p className="truncate text-sm font-medium text-(--text-h)">{n.title}</p>
                  </div>
                  <p className="text-xs text-(--text)">{n.message}</p>
                  <p className="text-[11px] text-(--text)">{timeAgo(n.createdAt)}</p>
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
