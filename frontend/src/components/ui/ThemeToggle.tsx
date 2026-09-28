import { useTheme } from "../../context/ThemeContext";
import { MoonIcon, SunIcon } from "./icons";

export function ThemeToggle({ className = "" }: { className?: string }) {
  const { theme, toggleTheme } = useTheme();
  const isDark = theme === "dark";

  return (
    <button
      type="button"
      onClick={toggleTheme}
      aria-label={isDark ? "Switch to light theme" : "Switch to dark theme"}
      title={isDark ? "Switch to light theme" : "Switch to dark theme"}
      className={`flex h-7 w-7 shrink-0 items-center justify-center rounded-(--radius) text-(--text) transition-colors hover:bg-(--accent-bg) hover:text-(--text-h) ${className}`}
    >
      {isDark ? <SunIcon className="h-4 w-4" /> : <MoonIcon className="h-4 w-4" />}
    </button>
  );
}
