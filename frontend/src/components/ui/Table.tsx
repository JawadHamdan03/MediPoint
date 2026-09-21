import type { ReactNode } from "react";

interface Column<T> {
  header: string;
  render: (row: T) => ReactNode;
}

interface TableProps<T> {
  columns: Column<T>[];
  rows: T[];
  rowKey: (row: T) => string;
  emptyMessage?: string;
}

export function Table<T>({ columns, rows, rowKey, emptyMessage = "No data." }: TableProps<T>) {
  if (rows.length === 0) {
    return (
      <div className="rounded-(--radius) border border-dashed border-(--border) py-10 text-center text-sm text-(--text)">
        {emptyMessage}
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-(--radius) border border-(--border) bg-(--surface) shadow-(--shadow)">
      <table className="w-full border-collapse text-left text-sm">
        <thead>
          <tr className="border-b border-(--border) bg-(--accent-bg)">
            {columns.map((col) => (
              <th key={col.header} className="whitespace-nowrap px-3 py-2.5 text-xs font-semibold uppercase tracking-wide text-(--text-h)">
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={rowKey(row)} className="border-b border-(--border) transition-colors duration-100 last:border-0 hover:bg-(--accent-bg)">
              {columns.map((col) => (
                <td key={col.header} className="px-3 py-2.5 text-(--text)">
                  {col.render(row)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
