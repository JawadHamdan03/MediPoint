import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { Button } from "../ui/Button";

interface NavItem {
  to: string;
  label: string;
}

export function AppShell({ navItems, title }: { navItems: NavItem[]; title: string }) {
  const auth = useAuth();

  return (
    <div className="flex min-h-screen">
      <aside className="flex w-56 flex-col gap-1 border-r border-(--border) p-4">
        <p className="mb-4 text-lg font-semibold text-(--text-h)">MediPoint</p>
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            end
            className={({ isActive }) =>
              `rounded-md px-3 py-2 text-sm font-medium ${
                isActive ? "bg-(--accent-bg) text-(--accent)" : "text-(--text) hover:bg-(--accent-bg)"
              }`
            }
          >
            {item.label}
          </NavLink>
        ))}
      </aside>

      <div className="flex flex-1 flex-col">
        <header className="flex items-center justify-between border-b border-(--border) px-6 py-3">
          <h1 className="text-lg font-semibold text-(--text-h)">{title}</h1>
          <div className="flex items-center gap-3 text-sm text-(--text)">
            <span>{auth.role}</span>
            <Button variant="secondary" onClick={auth.logout}>
              Log out
            </Button>
          </div>
        </header>
        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
