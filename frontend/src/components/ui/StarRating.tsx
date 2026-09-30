import { useState } from "react";
import { StarIcon } from "./icons";

interface StarRatingProps {
  value: number;
  onChange?: (value: number) => void;
  size?: "sm" | "md";
}

const sizeClasses = { sm: "h-4 w-4", md: "h-6 w-6" };

export function StarRating({ value, onChange, size = "sm" }: StarRatingProps) {
  const [hovered, setHovered] = useState<number | null>(null);
  const interactive = Boolean(onChange);
  const displayValue = hovered ?? value;

  return (
    <div className="flex items-center gap-0.5" onMouseLeave={() => setHovered(null)}>
      {[1, 2, 3, 4, 5].map((star) => (
        <button
          key={star}
          type="button"
          disabled={!interactive}
          onClick={() => onChange?.(star)}
          onMouseEnter={() => interactive && setHovered(star)}
          className={`${interactive ? "cursor-pointer" : "cursor-default"} text-amber-400 disabled:text-amber-400`}
          aria-label={`${star} star${star > 1 ? "s" : ""}`}
        >
          <StarIcon className={sizeClasses[size]} fill={star <= displayValue ? "currentColor" : "none"} />
        </button>
      ))}
    </div>
  );
}
