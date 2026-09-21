export function Skeleton({ className = "" }: { className?: string }) {
  return <div className={`animate-pulse rounded-(--radius) bg-(--accent-bg) ${className}`} />;
}

export function TableSkeleton({ rows = 4, cols = 4 }: { rows?: number; cols?: number }) {
  return (
    <div className="overflow-hidden rounded-(--radius) border border-(--border) bg-(--surface)">
      <div className="border-b border-(--border) bg-(--accent-bg) px-3 py-2.5">
        <Skeleton className="h-3 w-24" />
      </div>
      <div className="flex flex-col divide-y divide-(--border)">
        {Array.from({ length: rows }).map((_, r) => (
          <div key={r} className="flex gap-4 px-3 py-3">
            {Array.from({ length: cols }).map((_, c) => (
              <Skeleton key={c} className="h-4 flex-1" />
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}
