const palette = [
  "bg-purple-100 text-purple-700 dark:bg-purple-400/15 dark:text-purple-300",
  "bg-blue-100 text-blue-700 dark:bg-blue-400/15 dark:text-blue-300",
  "bg-green-100 text-green-700 dark:bg-green-400/15 dark:text-green-300",
  "bg-amber-100 text-amber-700 dark:bg-amber-400/15 dark:text-amber-300",
  "bg-pink-100 text-pink-700 dark:bg-pink-400/15 dark:text-pink-300",
];

function colorFor(seed: string): string {
  let hash = 0;
  for (let i = 0; i < seed.length; i++) hash = (hash * 31 + seed.charCodeAt(i)) >>> 0;
  return palette[hash % palette.length];
}

interface AvatarProps {
  name: string;
  size?: "sm" | "md" | "lg";
  imageUrl?: string | null;
}

const sizeClasses = {
  sm: "h-8 w-8 text-xs",
  md: "h-11 w-11 text-sm",
  lg: "h-20 w-20 text-xl",
};

export function Avatar({ name, size = "md", imageUrl }: AvatarProps) {
  const initials = name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase())
    .join("");

  if (imageUrl) {
    return (
      <img
        src={imageUrl}
        alt={name}
        className={`shrink-0 rounded-full border border-(--border) object-cover ${sizeClasses[size]}`}
      />
    );
  }

  return (
    <span
      className={`flex shrink-0 items-center justify-center rounded-full font-semibold ${sizeClasses[size]} ${colorFor(name || "?")}`}
    >
      {initials || "?"}
    </span>
  );
}
