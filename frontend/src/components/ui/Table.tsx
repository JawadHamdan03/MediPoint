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
    return <p className="py-6 text-center text-sm text-(--text)">{emptyMessage}</p>;
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-(--border)">
      <table className="w-full border-collapse text-left text-sm">
        <thead>
          <tr className="border-b border-(--border) bg-(--accent-bg)">
            {columns.map((col) => (
              <th key={col.header} className="px-3 py-2 font-medium text-(--text-h)">
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={rowKey(row)} className="border-b border-(--border) last:border-0">
              {columns.map((col) => (
                <td key={col.header} className="px-3 py-2 text-(--text)">
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
