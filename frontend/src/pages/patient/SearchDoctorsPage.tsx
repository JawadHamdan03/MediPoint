import { useEffect, useState } from "react";
import { bookAppointment, cancelAppointment, searchDoctors } from "../../api/patient";
import type { DoctorResponse } from "../../types/patient";
import type { AppointmentResponse } from "../../types/doctor";
import { useDebouncedValue } from "../../lib/useDebouncedValue";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { ConfirmButton } from "../../components/ui/ConfirmButton";
import { Card } from "../../components/ui/Card";
import { Badge } from "../../components/ui/Badge";
import { Skeleton } from "../../components/ui/Skeleton";
import { Avatar } from "../../components/ui/Avatar";
import { PageHeader } from "../../components/ui/PageHeader";
import { CalendarIcon, SearchIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

function DoctorCardSkeleton() {
  return (
    <Card>
      <Skeleton className="h-5 w-48" />
      <Skeleton className="mt-2 h-4 w-64" />
      <div className="mt-4 flex gap-2">
        <Skeleton className="h-6 w-32" />
        <Skeleton className="h-6 w-32" />
      </div>
    </Card>
  );
}

export default function SearchDoctorsPage() {
  const toast = useToast();
  const [speciality, setSpeciality] = useState("");
  const debouncedSpeciality = useDebouncedValue(speciality.trim(), 450);
  const [doctors, setDoctors] = useState<DoctorResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [bookingId, setBookingId] = useState<string | null>(null);
  // No "my appointments" endpoint exists yet, so track bookings made this
  // session locally to surface a Cancel action right after booking.
  const [booked, setBooked] = useState<AppointmentResponse[]>([]);

  useEffect(() => {
    if (!debouncedSpeciality) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setDoctors(null);
      return;
    }
    let cancelled = false;
    setLoading(true);
    setErrors([]);
    searchDoctors(debouncedSpeciality)
      .then((res) => {
        if (!cancelled) setDoctors(res);
      })
      .catch((err) => {
        if (!cancelled) setErrors(formatError(err));
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [debouncedSpeciality]);

  async function handleBook(appointmentId: string) {
    setErrors([]);
    setBookingId(appointmentId);
    try {
      const res = await bookAppointment(appointmentId);
      setBooked((b) => [...b, res]);
      toast.notify("Appointment booked.");
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setBookingId(null);
    }
  }

  async function handleCancel(appointmentId: string) {
    setErrors([]);
    try {
      await cancelAppointment(appointmentId, { cancellationReason: null });
      setBooked((b) => b.filter((a) => a.id !== appointmentId));
      toast.notify("Appointment cancelled.");
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <PageHeader icon={SearchIcon} title="Search Doctors" description="Results update as you type." />
      <div className="max-w-sm">
        <Input
          label="Speciality"
          placeholder="e.g. Cardiology"
          value={speciality}
          onChange={(e) => setSpeciality(e.target.value)}
        />
      </div>

      {errors.length > 0 && <ErrorList errors={errors} />}

      {booked.length > 0 && (
        <Card>
          <h3 className="mb-2 flex items-center gap-1.5 text-sm font-semibold text-(--text-h)">
            <CalendarIcon className="h-4 w-4 text-(--accent)" /> Booked this session
          </h3>
          <ul className="flex flex-col gap-3">
            {booked.map((a) => (
              <li key={a.id} className="flex flex-wrap items-center justify-between gap-2 text-sm text-(--text)">
                <span className="flex items-center gap-2">
                  {new Date(a.appointmentDate).toLocaleString()} <Badge status={a.status} />
                </span>
                <ConfirmButton label="Cancel" confirmLabel="Cancel this appointment?" onConfirm={() => handleCancel(a.id)} />
              </li>
            ))}
          </ul>
        </Card>
      )}

      {loading && (
        <div className="flex flex-col gap-4">
          <DoctorCardSkeleton />
          <DoctorCardSkeleton />
        </div>
      )}

      {!loading && doctors && (
        <div className="flex flex-col gap-4">
          {doctors.length === 0 && (
            <div className="flex flex-col items-center gap-2 rounded-(--radius) border border-dashed border-(--border) py-10 text-center">
              <SearchIcon className="h-6 w-6 text-(--text)" />
              <p className="text-sm text-(--text)">No doctors found for "{debouncedSpeciality}".</p>
            </div>
          )}
          {doctors.map((d) => (
            <Card key={d.licenseNumber}>
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div className="flex items-start gap-3">
                  <Avatar name={`${d.firstName} ${d.lastName}`} imageUrl={d.imageUrl} size="lg" />
                  <div>
                    <h3 className="text-base font-semibold text-(--text-h)">
                      Dr. {d.firstName} {d.lastName}
                    </h3>
                    <p className="text-sm text-(--text)">
                      {d.specialty} · {d.yearsOfExperience} yrs experience · ${d.consultationFee} consultation
                    </p>
                    {d.biography && <p className="mt-2 text-sm text-(--text)">{d.biography}</p>}
                  </div>
                </div>
              </div>

              <div className="mt-4">
                <p className="mb-2 flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-(--text-h)">
                  <CalendarIcon className="h-3.5 w-3.5" /> Available slots
                </p>
                {d.appointmentDTOs.length === 0 ? (
                  <p className="text-xs text-(--text)">No open slots.</p>
                ) : (
                  <ul className="flex flex-wrap gap-2">
                    {d.appointmentDTOs.map((a) => (
                      <li
                        key={a.id}
                        className="flex items-center gap-2 rounded-(--radius) border border-(--border) bg-(--bg) px-2.5 py-1.5 text-xs"
                      >
                        <span>{new Date(a.appointmentDate).toLocaleString()}</span>
                        <Badge status={a.status} />
                        {a.status === "Pending" && (
                          <Button
                            type="button"
                            className="!px-2 !py-0.5 !text-xs"
                            disabled={bookingId === a.id}
                            onClick={() => handleBook(a.id)}
                          >
                            {bookingId === a.id ? "Booking…" : "Book"}
                          </Button>
                        )}
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </Card>
          ))}
        </div>
      )}

      {!loading && !doctors && speciality.trim() === "" && (
        <div className="flex flex-col items-center gap-2 rounded-(--radius) border border-dashed border-(--border) py-12 text-center">
          <SearchIcon className="h-6 w-6 text-(--text)" />
          <p className="text-sm text-(--text)">Start typing a speciality to search for doctors.</p>
        </div>
      )}
    </div>
  );
}
