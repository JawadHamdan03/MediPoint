import type { SelectHTMLAttributes } from "react";

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  error?: string;
}

export function Select({ label, error, className = "", id, children, ...props }: SelectProps) {
  const selectId = id ?? label.toLowerCase().replace(/\s+/g, "-");
  return (
    <label htmlFor={selectId} className="flex flex-col gap-1 text-sm text-(--text)">
      {label}
      <select
        id={selectId}
        className={`rounded-md border bg-(--bg) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent) ${
          error ? "border-red-500" : "border-(--border)"
        } ${className}`}
        {...props}
      >
        {children}
      </select>
      {error && <span className="text-xs text-red-500">{error}</span>}
    </label>
  );
}
