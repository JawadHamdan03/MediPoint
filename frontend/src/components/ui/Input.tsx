import type { InputHTMLAttributes } from "react";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

export function Input({ label, error, className = "", id, ...props }: InputProps) {
  const inputId = id ?? label.toLowerCase().replace(/\s+/g, "-");
  return (
    <label htmlFor={inputId} className="flex flex-col gap-1 text-sm text-(--text)">
      {label}
      <input
        id={inputId}
        className={`rounded-md border px-3 py-2 text-(--text-h) outline-none focus:border-(--accent) ${
          error ? "border-red-500" : "border-(--border)"
        } ${className}`}
        {...props}
      />
      {error && <span className="text-xs text-red-500">{error}</span>}
    </label>
  );
}
