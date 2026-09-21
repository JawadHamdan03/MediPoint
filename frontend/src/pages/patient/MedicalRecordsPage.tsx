import { useEffect, useState } from "react";
import { getMedicalRecords } from "../../api/patient";
import type { MedicalRecordResponse } from "../../types/patient";
import { Card } from "../../components/ui/Card";
import { Skeleton } from "../../components/ui/Skeleton";
import { Avatar } from "../../components/ui/Avatar";
import { PageHeader } from "../../components/ui/PageHeader";
import { PillIcon, RecordsIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

function RecordCardSkeleton() {
  return (
    <Card>
      <Skeleton className="h-5 w-40" />
      <Skeleton className="mt-2 h-4 w-full max-w-md" />
      <Skeleton className="mt-4 h-4 w-24" />
    </Card>
  );
}

export default function MedicalRecordsPage() {
  const [records, setRecords] = useState<MedicalRecordResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  useEffect(() => {
    getMedicalRecords()
      .then(setRecords)
      .catch((err) => setErrors(formatError(err)));
  }, []);

  return (
    <div className="flex flex-col gap-4">
      <PageHeader icon={RecordsIcon} title="Medical Records" description="Prescriptions and lab results from your visits." />

      {errors.length > 0 && <ErrorList errors={errors} />}

      {records === null && errors.length === 0 && (
        <div className="flex flex-col gap-4">
          <RecordCardSkeleton />
          <RecordCardSkeleton />
        </div>
      )}

      {records !== null && records.length === 0 && (
        <div className="flex flex-col items-center gap-2 rounded-(--radius) border border-dashed border-(--border) py-10 text-center">
          <RecordsIcon className="h-6 w-6 text-(--text)" />
          <p className="text-sm text-(--text)">No medical records yet.</p>
        </div>
      )}

      {records?.map((r) => (
        <Card key={r.prescriptionId}>
          <div className="flex flex-wrap items-center justify-between gap-2">
            <h3 className="text-base font-semibold text-(--text-h)">{r.diagnosis || "Prescription"}</h3>
            <div className="flex items-center gap-2 text-sm text-(--text)">
              <Avatar name={r.doctorName} size="sm" />
              Dr. {r.doctorName}
            </div>
          </div>
          {r.notes && <p className="mt-1 text-sm text-(--text)">{r.notes}</p>}

          {r.medicines.length > 0 && (
            <div className="mt-3">
              <p className="mb-1 flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-(--text-h)">
                <PillIcon className="h-3.5 w-3.5 text-(--accent)" /> Medicines
              </p>
              <ul className="flex flex-col gap-1 text-sm text-(--text)">
                {r.medicines.map((m) => (
                  <li key={m.id ?? `${m.name}-${m.dosage}`}>
                    {m.name} — {m.dosage}, {m.frequency}, {m.durationDays} days
                    {m.instructions && ` (${m.instructions})`}
                  </li>
                ))}
              </ul>
            </div>
          )}

          {r.labResults.length > 0 && (
            <div className="mt-3">
              <p className="mb-1 flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-(--text-h)">
                <RecordsIcon className="h-3.5 w-3.5 text-(--accent)" /> Lab results
              </p>
              <ul className="flex flex-col gap-1 text-sm text-(--text)">
                {r.labResults.map((l) => (
                  <li key={l.id ?? `${l.testName}-${l.result}`}>
                    {l.testName}: {l.result} {l.unit} {l.referenceRange && `(ref: ${l.referenceRange})`}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </Card>
      ))}
    </div>
  );
}
