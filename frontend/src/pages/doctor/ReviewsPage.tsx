import { useEffect, useState } from "react";
import { getMyReviews } from "../../api/doctor";
import type { ReviewResponse } from "../../types/doctor";
import { Card } from "../../components/ui/Card";
import { StarRating } from "../../components/ui/StarRating";
import { TableSkeleton } from "../../components/ui/Skeleton";
import { PageHeader } from "../../components/ui/PageHeader";
import { StarIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

export default function DoctorReviewsPage() {
  const [reviews, setReviews] = useState<ReviewResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  useEffect(() => {
    getMyReviews()
      .then(setReviews)
      .catch((err) => setErrors(formatError(err)));
  }, []);

  const average =
    reviews && reviews.length > 0 ? reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length : null;

  return (
    <div className="flex flex-col gap-4">
      <PageHeader
        icon={StarIcon}
        title="My Reviews"
        description={average !== null ? `${average.toFixed(1)} average · ${reviews!.length} review${reviews!.length === 1 ? "" : "s"}` : "Patient feedback on your completed appointments."}
      />

      {errors.length > 0 && <ErrorList errors={errors} />}

      {reviews === null ? (
        <TableSkeleton rows={3} cols={2} />
      ) : reviews.length === 0 ? (
        <div className="flex flex-col items-center gap-2 rounded-(--radius) border border-dashed border-(--border) py-10 text-center">
          <StarIcon className="h-6 w-6 text-(--text)" />
          <p className="text-sm text-(--text)">No reviews yet.</p>
        </div>
      ) : (
        <div className="flex flex-col gap-3">
          {reviews.map((r) => (
            <Card key={r.id}>
              <div className="flex items-center justify-between gap-2">
                <p className="text-sm font-medium text-(--text-h)">{r.patientName}</p>
                <StarRating value={r.rating} />
              </div>
              {r.comment && <p className="mt-2 text-sm text-(--text)">{r.comment}</p>}
              <p className="mt-1 text-xs text-(--text)">{new Date(r.createdAt).toLocaleDateString()}</p>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
