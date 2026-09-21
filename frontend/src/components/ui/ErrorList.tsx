export function ErrorList({ errors }: { errors: string[] }) {
  if (errors.length === 0) return null;
  return (
    <ul className="flex flex-col gap-1 rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-700">
      {errors.map((e) => (
        <li key={e}>{e}</li>
      ))}
    </ul>
  );
}
