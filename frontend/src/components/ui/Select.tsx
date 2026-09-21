import type { SelectHTMLAttributes } from "react";

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  error?: string;
}

export function Select({ label, error, className = "", id, children, ...props }: SelectProps) {
  const selectId = id ?? label.toLowerCase().replace(/\s+/g, "-");
  return (
    <label htmlFor={selectId} className="flex flex-col gap-1.5 text-sm font-medium text-(--text)">
      {label}
      <select
        id={selectId}
        className={`rounded-(--radius) border bg-(--surface) px-3 py-2 text-sm font-normal text-(--text-h) outline-none transition-colors duration-150 focus:border-(--accent) focus:ring-2 focus:ring-(--accent-bg) ${
          error ? "border-red-400" : "border-(--border)"
        } ${className}`}
        {...props}
      >
        {children}
      </select>
      {error && <span className="text-xs font-normal text-red-500">{error}</span>}
    </label>
  );
}
