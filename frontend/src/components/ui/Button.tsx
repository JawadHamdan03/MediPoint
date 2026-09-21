import type { ButtonHTMLAttributes } from "react";

type Variant = "primary" | "secondary" | "danger" | "ghost";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
}

const variantClasses: Record<Variant, string> = {
  primary: "bg-(--accent) text-white hover:bg-(--accent-hover) shadow-sm",
  secondary: "border border-(--border) bg-(--surface) text-(--text-h) hover:bg-(--accent-bg) hover:border-(--accent-border)",
  danger: "bg-red-600 text-white hover:bg-red-700 shadow-sm",
  ghost: "text-(--text) hover:bg-(--accent-bg) hover:text-(--text-h)",
};

export function Button({ variant = "primary", className = "", disabled, ...props }: ButtonProps) {
  return (
    <button
      disabled={disabled}
      className={`inline-flex items-center justify-center gap-1.5 rounded-(--radius) px-3.5 py-2 text-sm font-medium transition-colors duration-150 disabled:cursor-not-allowed disabled:opacity-50 ${variantClasses[variant]} ${className}`}
      {...props}
    />
  );
}
