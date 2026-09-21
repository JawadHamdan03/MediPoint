import type { HTMLAttributes } from "react";

export function Card({ className = "", ...props }: HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={`rounded-(--radius) border border-(--border) bg-(--surface) p-5 shadow-(--shadow) transition-colors duration-150 ${className}`}
      {...props}
    />
  );
}
