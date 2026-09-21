import { useState } from "react";
import { removeDoctor } from "../../api/admin";
import { Input } from "../../components/ui/Input";
import { ConfirmButton } from "../../components/ui/ConfirmButton";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { UserMinusIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

export default function RemoveDoctorPage() {
  const toast = useToast();
  const [doctorId, setDoctorId] = useState("");
  const [errors, setErrors] = useState<string[]>([]);

  async function handleRemove() {
    setErrors([]);
    try {
      const res = await removeDoctor(doctorId);
      toast.notify(`Doctor ${res.firstName} ${res.lastName} deactivated.`);
      setDoctorId("");
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  return (
    <div className="mx-auto max-w-md">
      <PageHeader
        icon={UserMinusIcon}
        title="Remove Doctor"
        description="This deactivates the doctor (soft delete) rather than deleting the record."
      />
      <Card>
        <div className="flex flex-col gap-4">
          <Input label="Doctor id" required value={doctorId} onChange={(e) => setDoctorId(e.target.value)} />
          {errors.length > 0 && <ErrorList errors={errors} />}
          <div>
            <ConfirmButton label="Remove doctor" confirmLabel="Deactivate this doctor?" disabled={!doctorId} onConfirm={handleRemove} />
          </div>
        </div>
      </Card>
    </div>
  );
}
