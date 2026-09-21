import { useState, type FormEvent } from "react";
import { bookAppointment, cancelAppointment, searchDoctors } from "../../api/patient";
import type { DoctorResponse } from "../../types/patient";
import type { AppointmentResponse } from "../../types/doctor";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { Badge } from "../../components/ui/Badge";
import { Spinner } from "../../components/ui/Spinner";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

export default function SearchDoctorsPage() {
  const [speciality, setSpeciality] = useState("");
  const [doctors, setDoctors] = useState<DoctorResponse[] | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  // No "my appointments" endpoint exists yet, so track bookings made this
  // session locally to surface a Cancel action right after booking.
  const [booked, setBooked] = useState<AppointmentResponse[]>([]);

  async function handleSearch(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setLoading(true);
    try {
      setDoctors(await searchDoctors(speciality));
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setLoading(false);
    }
  }

  async function handleBook(appointmentId: string) {
    setErrors([]);
    try {
      const res = await bookAppointment(appointmentId);
      setBooked((b) => [...b, res]);
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  async function handleCancel(appointmentId: string) {
    setErrors([]);
    try {
      await cancelAppointment(appointmentId, { cancellationReason: null });
      setBooked((b) => b.filter((a) => a.id !== appointmentId));
    } catch (err) {
      setErrors(formatError(err));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <Card>
        <h2 className="mb-4 text-lg font-semibold text-(--text-h)">Search Doctors</h2>
        <form onSubmit={handleSearch} className="flex items-end gap-3">
          <div className="flex-1">
            <Input label="Speciality" required value={speciality} onChange={(e) => setSpeciality(e.target.value)} />
          </div>
          <Button type="submit" disabled={loading}>
            {loading ? "Searching…" : "Search"}
          </Button>
        </form>
      </Card>

      {errors.length > 0 && <ErrorList errors={errors} />}

      {booked.length > 0 && (
        <Card>
          <h3 className="mb-2 text-sm font-semibold text-(--text-h)">Booked this session</h3>
          <ul className="flex flex-col gap-2">
            {booked.map((a) => (
              <li key={a.id} className="flex items-center justify-between text-sm text-(--text)">
                <span>
                  {new Date(a.appointmentDate).toLocaleString()} · <Badge status={a.status} />
                </span>
                <Button variant="danger" onClick={() => handleCancel(a.id)}>
                  Cancel
                </Button>
              </li>
            ))}
          </ul>
        </Card>
      )}

      {loading && <Spinner />}

      {doctors && (
        <div className="flex flex-col gap-4">
          {doctors.length === 0 && <p className="text-sm text-(--text)">No doctors found for that speciality.</p>}
          {doctors.map((d) => (
            <Card key={d.licenseNumber}>
              <div className="flex items-start justify-between">
                <div>
                  <h3 className="text-base font-semibold text-(--text-h)">
                    Dr. {d.firstName} {d.lastName}
                  </h3>
                  <p className="text-sm text-(--text)">
                    {d.specialty} · {d.yearsOfExperience} yrs · ${d.consultationFee}
                  </p>
                  {d.biography && <p className="mt-1 text-sm text-(--text)">{d.biography}</p>}
                </div>
              </div>

              <div className="mt-3">
                <p className="mb-1 text-xs font-medium text-(--text-h)">Available slots</p>
                {d.appointmentDTOs.length === 0 ? (
                  <p className="text-xs text-(--text)">No open slots.</p>
                ) : (
                  <ul className="flex flex-wrap gap-2">
                    {d.appointmentDTOs.map((a) => (
                      <li key={a.id} className="flex items-center gap-2 rounded-md border border-(--border) px-2 py-1 text-xs">
                        <span>{new Date(a.appointmentDate).toLocaleString()}</span>
                        <Badge status={a.status} />
                        {a.status === "Pending" && (
                          <button
                            type="button"
                            onClick={() => handleBook(a.id)}
                            className="text-(--accent) underline"
                          >
                            Book
                          </button>
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
    </div>
  );
}
