import type { ComponentType, ReactNode, SVGProps } from "react";

interface PageHeaderProps {
  icon: ComponentType<SVGProps<SVGSVGElement>>;
  title: string;
  description?: string;
  action?: ReactNode;
}

export function PageHeader({ icon: Icon, title, description, action }: PageHeaderProps) {
  return (
    <div className="mb-5 flex flex-wrap items-start justify-between gap-3">
      <div className="flex items-start gap-3">
        <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-(--radius) bg-(--accent-bg) text-(--accent)">
          <Icon className="h-5 w-5" />
        </span>
        <div>
          <h2 className="text-lg font-semibold text-(--text-h)">{title}</h2>
          {description && <p className="mt-0.5 text-sm text-(--text)">{description}</p>}
        </div>
      </div>
      {action}
    </div>
  );
}
