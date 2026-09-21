import type { TextareaHTMLAttributes } from "react";

interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label: string;
  error?: string;
}

export function Textarea({ label, error, className = "", id, ...props }: TextareaProps) {
  const areaId = id ?? label.toLowerCase().replace(/\s+/g, "-");
  return (
    <label htmlFor={areaId} className="flex flex-col gap-1.5 text-sm font-medium text-(--text)">
      {label}
      <textarea
        id={areaId}
        className={`rounded-(--radius) border bg-(--surface) px-3 py-2 text-sm font-normal text-(--text-h) outline-none transition-colors duration-150 placeholder:text-(--text)/50 focus:border-(--accent) focus:ring-2 focus:ring-(--accent-bg) ${
          error ? "border-red-400" : "border-(--border)"
        } ${className}`}
        {...props}
      />
      {error && <span className="text-xs font-normal text-red-500">{error}</span>}
    </label>
  );
}
