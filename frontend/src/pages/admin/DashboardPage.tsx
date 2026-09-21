import { Link } from "react-router-dom";
import { Card } from "../../components/ui/Card";
import { UserPlusIcon, UserEditIcon, UserMinusIcon } from "../../components/ui/icons";

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

export default function DashboardPage() {
  return (
    <div className="flex flex-col gap-6">
      <div>
        <h2 className="text-xl font-semibold text-(--text-h)">Welcome back</h2>
        <p className="mt-1 text-sm text-(--text)">Manage doctors and patients from the actions below.</p>
      </div>

      <div className="flex items-start gap-3 rounded-(--radius) border border-(--accent-border) bg-(--accent-bg) px-4 py-3 text-sm text-(--text-h)">
        <span className="mt-0.5 text-(--accent)">ⓘ</span>
        <p>No "list all doctors" endpoint exists yet, so there's no live doctor table here — use the actions below with a known doctor id.</p>
      </div>

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
