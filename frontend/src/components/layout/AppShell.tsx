import type { ComponentType, SVGProps } from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { Button } from "../ui/Button";
import { Avatar } from "../ui/Avatar";
import { LogoutIcon } from "../ui/icons";

export interface NavItem {
  to: string;
  label: string;
  icon: ComponentType<SVGProps<SVGSVGElement>>;
}

const roleBadgeClasses: Record<string, string> = {
  Admin: "bg-purple-100 text-purple-800 dark:bg-purple-400/15 dark:text-purple-300",
  Doctor: "bg-blue-100 text-blue-800 dark:bg-blue-400/15 dark:text-blue-300",
  Patient: "bg-green-100 text-green-800 dark:bg-green-400/15 dark:text-green-300",
};

function NavLinks({ navItems, onNavigate }: { navItems: NavItem[]; onNavigate?: () => void }) {
  return (
    <>
      {navItems.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          end
          onClick={onNavigate}
          className={({ isActive }) =>
            `flex items-center gap-2.5 rounded-(--radius) px-3 py-2 text-sm font-medium transition-colors duration-150 whitespace-nowrap ${
              isActive
                ? "bg-(--accent) text-white shadow-sm"
                : "text-(--text) hover:bg-(--accent-bg) hover:text-(--text-h)"
            }`
          }
        >
          <item.icon className="h-4.5 w-4.5 shrink-0" />
          {item.label}
        </NavLink>
      ))}
    </>
  );
}

export function AppShell({ navItems, title }: { navItems: NavItem[]; title: string }) {
  const auth = useAuth();
  const fullName = [auth.firstName, auth.lastName].filter(Boolean).join(" ");
  const displayName = fullName || auth.role || "";

  return (
    <div className="flex min-h-screen bg-(--bg)">
      <aside className="sticky top-0 hidden h-screen w-60 shrink-0 flex-col gap-1 border-r border-(--border) bg-(--surface) p-4 md:flex">
        <div className="mb-6 flex items-center gap-2 px-1">
          <span className="flex h-8 w-8 items-center justify-center rounded-(--radius) bg-(--accent) text-sm font-bold text-white">
            M
          </span>
          <p className="text-lg font-semibold text-(--text-h)">MediPoint</p>
        </div>
        <nav className="flex flex-col gap-1">
          <NavLinks navItems={navItems} />
        </nav>

        <div className="mt-auto flex items-center gap-2.5 rounded-(--radius) border border-(--border) p-2.5">
          <Avatar name={displayName} imageUrl={auth.imageUrl} size="sm" />
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-(--text-h)">{displayName}</p>
            <p className="truncate text-xs text-(--text)">Signed in</p>
          </div>
          <button
            type="button"
            onClick={auth.logout}
            aria-label="Log out"
            className="flex h-7 w-7 shrink-0 items-center justify-center rounded-(--radius) text-(--text) transition-colors hover:bg-(--accent-bg) hover:text-(--text-h)"
          >
            <LogoutIcon className="h-4 w-4" />
          </button>
        </div>
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="sticky top-0 z-10 flex items-center justify-between gap-4 border-b border-(--border) bg-(--surface)/90 px-4 py-3 backdrop-blur sm:px-6">
          <div className="flex items-center gap-2">
            <span className="flex h-7 w-7 items-center justify-center rounded-(--radius) bg-(--accent) text-xs font-bold text-white md:hidden">
              M
            </span>
            <h1 className="text-lg font-semibold text-(--text-h)">{title}</h1>
          </div>
          <div className="flex items-center gap-3">
            <span className={`hidden rounded-full px-2.5 py-1 text-xs font-medium sm:inline-block ${roleBadgeClasses[auth.role ?? ""] ?? ""}`}>
              {auth.role}
            </span>
            <Button variant="secondary" onClick={auth.logout} className="md:hidden">
              <LogoutIcon className="h-4 w-4" />
            </Button>
          </div>
        </header>

        <nav className="flex gap-1 overflow-x-auto border-b border-(--border) bg-(--surface) px-4 py-2 md:hidden">
          <NavLinks navItems={navItems} />
        </nav>

        <main className="flex-1 p-4 sm:p-6" style={{ animation: "fade-in 0.15s ease-out" }}>
          <Outlet />
        </main>
      </div>
    </div>
  );
}
