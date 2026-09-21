import { useState, type ReactNode } from "react";
import { Button } from "./Button";

interface ConfirmButtonProps {
  label: ReactNode;
  confirmLabel?: string;
  variant?: "primary" | "secondary" | "danger";
  disabled?: boolean;
  onConfirm: () => void | Promise<void>;
}

export function ConfirmButton({ label, confirmLabel = "Confirm?", variant = "danger", disabled, onConfirm }: ConfirmButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const [busy, setBusy] = useState(false);

  if (!confirming) {
    return (
      <Button type="button" variant={variant} disabled={disabled} onClick={() => setConfirming(true)}>
        {label}
      </Button>
    );
  }

  return (
    <span className="inline-flex items-center gap-2">
      <span className="text-sm text-(--text)">{confirmLabel}</span>
      <Button
        type="button"
        variant="danger"
        disabled={busy}
        onClick={async () => {
          setBusy(true);
          try {
            await onConfirm();
          } finally {
            setBusy(false);
            setConfirming(false);
          }
        }}
      >
        {busy ? "…" : "Yes"}
      </Button>
      <Button type="button" variant="secondary" disabled={busy} onClick={() => setConfirming(false)}>
        No
      </Button>
    </span>
  );
}
