import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getDashboard } from "../../api/admin";
import type { AdminDashboardResponse, DailyAppointmentCount } from "../../types/admin";
import { Card } from "../../components/ui/Card";
import { Skeleton } from "../../components/ui/Skeleton";
import { StarRating } from "../../components/ui/StarRating";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import {
  CalendarIcon,
  CheckIcon,
  DollarIcon,
  StarIcon,
  UserEditIcon,
  UserMinusIcon,
  UserPlusIcon,
  XCircleIcon,
} from "../../components/ui/icons";

const links = [
  {
    to: "/admin/doctors/add",
    label: "Add Doctor",
    desc: "Onboard a new doctor account.",
    icon: UserPlusIcon,
    accent: "bg-purple-100 text-purple-700 dark:bg-purple-400/15 dark:text-purple-300",
  },
  {
    to: "/admin/doctors/update",
    label: "Update Doctor",
    desc: "Edit an existing doctor's profile.",
    icon: UserEditIcon,
    accent: "bg-blue-100 text-blue-700 dark:bg-blue-400/15 dark:text-blue-300",
  },
  {
    to: "/admin/doctors/remove",
    label: "Remove Doctor",
    desc: "Deactivate a doctor (soft delete).",
    icon: UserMinusIcon,
    accent: "bg-red-100 text-red-700 dark:bg-red-400/15 dark:text-red-300",
  },
  {
    to: "/admin/patients/register",
    label: "Register Patient",
    desc: "Create a new patient account.",
    icon: UserPlusIcon,
    accent: "bg-green-100 text-green-700 dark:bg-green-400/15 dark:text-green-300",
  },
];

function formatCompact(n: number): string {
  if (n >= 1_000_000) return `${(n / 1_000_000).toFixed(1)}M`;
  if (n >= 1_000) return `${(n / 1_000).toFixed(1)}K`;
  return n.toLocaleString();
}

function formatCurrency(n: number): string {
  return `$${formatCompact(n)}`;
}

function StatTile({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof CalendarIcon;
  label: string;
  value: string;
}) {
  return (
    <Card className="flex items-center gap-3">
      <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-(--radius) bg-(--accent-bg) text-(--accent)">
        <Icon className="h-5 w-5" />
      </span>
      <div className="min-w-0">
        <p className="truncate text-xs text-(--text)">{label}</p>
        <p className="text-xl font-semibold text-(--text-h)">{value}</p>
      </div>
    </Card>
  );
}

function TrendChart({ data }: { data: DailyAppointmentCount[] }) {
  const [hovered, setHovered] = useState<number | null>(null);
  if (data.length === 0) {
    return <p className="py-8 text-center text-sm text-(--text)">No appointment activity in the last 30 days.</p>;
  }

  const width = 600;
  const height = 140;
  const gap = 2;
  const barWidth = Math.max(2, width / data.length - gap);
  const maxCount = Math.max(1, ...data.map((d) => d.count));
  const plotHeight = 110;

  return (
    <div className="relative">
      <svg viewBox={`0 0 ${width} ${height}`} className="w-full" style={{ height: "auto" }} role="img" aria-label="Appointments created per day, last 30 days">
        <line x1={0} y1={plotHeight + 8} x2={width} y2={plotHeight + 8} stroke="var(--border)" strokeWidth={1} />
        {data.map((d, i) => {
          const x = i * (barWidth + gap);
          const barHeight = Math.max(1, (d.count / maxCount) * plotHeight);
          const y = plotHeight + 8 - barHeight;
          const isHovered = hovered === i;
          return (
            <rect
              key={d.date}
              x={x}
              y={y}
              width={barWidth}
              height={barHeight}
              rx={2}
              fill="var(--accent)"
              opacity={isHovered ? 1 : 0.85}
              tabIndex={0}
              onMouseEnter={() => setHovered(i)}
              onMouseLeave={() => setHovered(null)}
              onFocus={() => setHovered(i)}
              onBlur={() => setHovered(null)}
            />
          );
        })}
        {data.map((d, i) =>
          i % 5 === 0 ? (
            <text
              key={`label-${d.date}`}
              x={i * (barWidth + gap) + barWidth / 2}
              y={height - 4}
              fontSize={9}
              textAnchor="middle"
              fill="var(--text)"
            >
              {new Date(d.date).toLocaleDateString(undefined, { month: "short", day: "numeric" })}
            </text>
          ) : null,
        )}
      </svg>
      {hovered !== null && (
        <div
          className="pointer-events-none absolute -top-1 rounded-(--radius) border border-(--border) bg-(--surface) px-2 py-1 text-xs shadow-(--shadow)"
          style={{ left: `${(hovered / data.length) * 100}%`, transform: "translate(-50%, -100%)" }}
        >
          <p className="font-semibold text-(--text-h)">{data[hovered].count}</p>
          <p className="text-(--text)">{new Date(data[hovered].date).toLocaleDateString()}</p>
        </div>
      )}
    </div>
  );
}

function RevenueBars({ data }: { data: AdminDashboardResponse["revenueBySpecialty"] }) {
  if (data.length === 0) {
    return <p className="py-8 text-center text-sm text-(--text)">No completed appointments yet.</p>;
  }

  const max = Math.max(...data.map((d) => d.revenue));

  return (
    <div className="flex flex-col gap-2.5">
      {data.map((d) => (
        <div key={d.specialty} className="flex items-center gap-3">
          <span className="w-28 shrink-0 truncate text-sm text-(--text-h)">{d.specialty}</span>
          <div className="h-4 flex-1 rounded-full bg-(--accent-bg)">
            <div
              className="h-4 rounded-full bg-(--accent)"
              style={{ width: `${max === 0 ? 0 : (d.revenue / max) * 100}%` }}
            />
          </div>
          <span className="w-16 shrink-0 text-right text-sm font-medium text-(--text-h)">{formatCurrency(d.revenue)}</span>
        </div>
      ))}
    </div>
  );
}

export default function DashboardPage() {
  const [dashboard, setDashboard] = useState<AdminDashboardResponse | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  useEffect(() => {
    getDashboard()
      .then(setDashboard)
      .catch((err) => setErrors(formatError(err)));
  }, []);

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h2 className="text-xl font-semibold text-(--text-h)">Welcome back</h2>
        <p className="mt-1 text-sm text-(--text)">An overview of appointment activity across the platform.</p>
      </div>

      {errors.length > 0 && <ErrorList errors={errors} />}

      {dashboard === null && errors.length === 0 ? (
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <Card key={i}>
              <Skeleton className="h-10 w-full" />
            </Card>
          ))}
        </div>
      ) : dashboard ? (
        <>
          <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
            <StatTile icon={CalendarIcon} label="Total appointments" value={formatCompact(dashboard.totalAppointments)} />
            <StatTile icon={CheckIcon} label="Completion rate" value={`${(dashboard.completionRate * 100).toFixed(0)}%`} />
            <StatTile icon={XCircleIcon} label="Cancellation rate" value={`${(dashboard.cancellationRate * 100).toFixed(0)}%`} />
            <StatTile icon={DollarIcon} label="Total revenue" value={formatCurrency(dashboard.totalRevenue)} />
          </div>

          <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
            <Card>
              <h3 className="mb-3 text-sm font-semibold text-(--text-h)">Appointment volume · last 30 days</h3>
              <TrendChart data={dashboard.appointmentsLast30Days} />
            </Card>

            <Card>
              <h3 className="mb-3 text-sm font-semibold text-(--text-h)">Revenue by specialty</h3>
              <RevenueBars data={dashboard.revenueBySpecialty} />
            </Card>
          </div>

          <Card>
            <h3 className="mb-3 flex items-center gap-1.5 text-sm font-semibold text-(--text-h)">
              <StarIcon className="h-4 w-4 text-(--accent)" /> Top rated doctors
            </h3>
            {dashboard.topRatedDoctors.length === 0 ? (
              <p className="text-sm text-(--text)">No reviews yet.</p>
            ) : (
              <ul className="flex flex-col divide-y divide-(--border)">
                {dashboard.topRatedDoctors.map((d) => (
                  <li key={d.doctorId} className="flex items-center justify-between gap-3 py-2.5 first:pt-0 last:pb-0">
                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium text-(--text-h)">{d.name}</p>
                      <p className="text-xs text-(--text)">{d.specialty}</p>
                    </div>
                    <div className="flex shrink-0 items-center gap-2">
                      <StarRating value={Math.round(d.averageRating)} />
                      <span className="text-xs text-(--text)">
                        {d.averageRating.toFixed(1)} ({d.reviewCount})
                      </span>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </>
      ) : null}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {links.map((l) => (
          <Link key={l.to} to={l.to}>
            <Card className="flex h-full items-start gap-4 transition-shadow duration-150 hover:border-(--accent-border) hover:shadow-md">
              <span className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-(--radius) ${l.accent}`}>
                <l.icon className="h-5 w-5" />
              </span>
              <div>
                <h3 className="text-base font-semibold text-(--text-h)">{l.label}</h3>
                <p className="mt-1 text-sm text-(--text)">{l.desc}</p>
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
