import { useEffect, useState } from "react";
import { getMedicalRecords } from "../../api/patient";
import type { MedicalRecordResponse } from "../../types/patient";
import { Card } from "../../components/ui/Card";
import { Spinner } from "../../components/ui/Spinner";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

export default function MedicalRecordsPage() {
  const [records, setRecords] = useState<MedicalRecordResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  useEffect(() => {
    getMedicalRecords()
      .then(setRecords)
      .catch((err) => setErrors(formatError(err)));
  }, []);

  if (errors.length > 0) return <ErrorList errors={errors} />;
  if (records === null) return <Spinner />;
  if (records.length === 0) return <p className="text-sm text-(--text)">No medical records yet.</p>;

  return (
    <div className="flex flex-col gap-4">
      <h2 className="text-lg font-semibold text-(--text-h)">Medical Records</h2>
      {records.map((r) => (
        <Card key={r.prescriptionId}>
          <div className="flex items-center justify-between">
            <h3 className="text-base font-semibold text-(--text-h)">{r.diagnosis || "Prescription"}</h3>
            <span className="text-sm text-(--text)">Dr. {r.doctorName}</span>
          </div>
          {r.notes && <p className="mt-1 text-sm text-(--text)">{r.notes}</p>}

          {r.medicines.length > 0 && (
            <div className="mt-3">
              <p className="mb-1 text-xs font-medium text-(--text-h)">Medicines</p>
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
              <p className="mb-1 text-xs font-medium text-(--text-h)">Lab results</p>
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
