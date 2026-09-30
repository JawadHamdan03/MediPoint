import { useEffect, useState } from "react";
import { addReview, cancelAppointment, getMyAppointments } from "../../api/patient";
import type { MyAppointmentResponse } from "../../types/patient";
import { Card } from "../../components/ui/Card";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { ConfirmButton } from "../../components/ui/ConfirmButton";
import { Textarea } from "../../components/ui/Textarea";
import { StarRating } from "../../components/ui/StarRating";
import { TableSkeleton } from "../../components/ui/Skeleton";
import { PageHeader } from "../../components/ui/PageHeader";
import { CalendarIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

function ReviewForm({ appointment, onSubmitted }: { appointment: MyAppointmentResponse; onSubmitted: () => void }) {
  const toast = useToast();
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  async function handleSubmit() {
    setSubmitting(true);
    setErrors([]);
    try {
      await addReview(appointment.id, { rating, comment: comment || null });
      toast.notify("Review submitted.");
      onSubmitted();
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mt-3 flex flex-col gap-3 rounded-(--radius) border border-(--border) bg-(--bg) p-3">
      <div className="flex items-center gap-2">
        <span className="text-sm font-medium text-(--text-h)">Your rating</span>
        <StarRating value={rating} onChange={setRating} size="md" />
      </div>
      <Textarea
        label="Comment (optional)"
        maxLength={1000}
        rows={2}
        value={comment}
        onChange={(e) => setComment(e.target.value)}
      />
      {errors.length > 0 && <ErrorList errors={errors} />}
      <Button type="button" onClick={handleSubmit} disabled={submitting} className="self-start">
        {submitting ? "Submitting…" : "Submit review"}
      </Button>
    </div>
  );
}

export default function MyAppointmentsPage() {
  const toast = useToast();
  const [appointments, setAppointments] = useState<MyAppointmentResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [reviewingId, setReviewingId] = useState<string | null>(null);

  async function load() {
    setErrors([]);
    try {
      setAppointments(await getMyAppointments());
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  useEffect(() => {
    // Standard fetch-on-mount: load() resets error state before the request.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, []);

  async function handleCancel(appointmentId: string) {
    try {
      await cancelAppointment(appointmentId, { cancellationReason: null });
      toast.notify("Appointment cancelled.");
      load();
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  return (
    <div className="flex flex-col gap-4">
      <PageHeader icon={CalendarIcon} title="My Appointments" description="Your appointment history and upcoming visits." />

      {errors.length > 0 && <ErrorList errors={errors} />}

      {appointments === null ? (
        <TableSkeleton rows={4} cols={4} />
      ) : appointments.length === 0 ? (
        <div className="flex flex-col items-center gap-2 rounded-(--radius) border border-dashed border-(--border) py-10 text-center">
          <CalendarIcon className="h-6 w-6 text-(--text)" />
          <p className="text-sm text-(--text)">You have no appointments yet.</p>
        </div>
      ) : (
        <div className="flex flex-col gap-3">
          {appointments.map((a) => (
            <Card key={a.id}>
              <div className="flex flex-wrap items-center justify-between gap-2">
                <div>
                  <p className="text-sm font-semibold text-(--text-h)">{a.doctorName}</p>
                  <p className="text-xs text-(--text)">{new Date(a.appointmentDate).toLocaleString()}</p>
                </div>
                <div className="flex items-center gap-2">
                  <Badge status={a.status} />
                  {(a.status === "Pending" || a.status === "Confirmed") && (
                    <ConfirmButton label="Cancel" confirmLabel="Cancel this appointment?" onConfirm={() => handleCancel(a.id)} />
                  )}
                  {a.status === "Completed" && a.hasReview && (
                    <span className="text-xs font-medium text-(--text)">Reviewed ✓</span>
                  )}
                  {a.status === "Completed" && !a.hasReview && reviewingId !== a.id && (
                    <Button type="button" variant="secondary" onClick={() => setReviewingId(a.id)}>
                      Leave a review
                    </Button>
                  )}
                </div>
              </div>
              {a.reason && <p className="mt-2 text-sm text-(--text)">{a.reason}</p>}
              {a.notes && <p className="mt-1 text-xs text-(--text)">Notes: {a.notes}</p>}
              {reviewingId === a.id && (
                <ReviewForm
                  appointment={a}
                  onSubmitted={() => {
                    setReviewingId(null);
                    load();
                  }}
                />
              )}
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
