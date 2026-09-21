import type { ButtonHTMLAttributes } from "react";

type Variant = "primary" | "secondary" | "danger";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
}

const variantClasses: Record<Variant, string> = {
  primary: "bg-(--accent) text-white hover:opacity-90",
  secondary: "border border-(--border) text-(--text-h) hover:bg-(--accent-bg)",
  danger: "bg-red-600 text-white hover:opacity-90",
};

export function Button({ variant = "primary", className = "", disabled, ...props }: ButtonProps) {
  return (
    <button
      disabled={disabled}
      className={`rounded-md px-3 py-2 text-sm font-medium transition-opacity disabled:opacity-60 disabled:cursor-not-allowed ${variantClasses[variant]} ${className}`}
      {...props}
    />
  );
}
