import { Link } from "react-router-dom";
import { Card } from "../../components/ui/Card";

const links = [
  { to: "/admin/doctors/add", label: "Add Doctor", desc: "Onboard a new doctor account." },
  { to: "/admin/doctors/update", label: "Update Doctor", desc: "Edit an existing doctor's profile." },
  { to: "/admin/doctors/remove", label: "Remove Doctor", desc: "Deactivate a doctor (soft delete)." },
  { to: "/admin/patients/register", label: "Register Patient", desc: "Create a new patient account." },
];

export default function DashboardPage() {
  return (
    <div className="flex flex-col gap-4">
      <p className="text-sm text-(--text)">
        No "list all doctors" endpoint exists yet, so there's no live doctor table here — use the actions below
        with a known doctor id.
      </p>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {links.map((l) => (
          <Link key={l.to} to={l.to}>
            <Card className="h-full hover:border-(--accent)">
              <h2 className="text-base font-semibold text-(--text-h)">{l.label}</h2>
              <p className="mt-1 text-sm text-(--text)">{l.desc}</p>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
