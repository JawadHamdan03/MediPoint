import { useState, type FormEvent } from "react";
import { generateAppointmentSlots } from "../../api/doctor";
import type { DayOfWeekName } from "../../types/doctor";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { RepeatIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { useToast } from "../../context/ToastContext";

const DAYS: DayOfWeekName[] = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

export default function GenerateSlotsPage() {
  const toast = useToast();
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [daysOfWeek, setDaysOfWeek] = useState<DayOfWeekName[]>(["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]);
  const [startTime, setStartTime] = useState("09:00");
  const [endTime, setEndTime] = useState("17:00");
  const [slotDurationMinutes, setSlotDurationMinutes] = useState(30);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function toggleDay(day: DayOfWeekName) {
    setDaysOfWeek((days) => (days.includes(day) ? days.filter((d) => d !== day) : [...days, day]));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      const result = await generateAppointmentSlots({
        startDate,
        endDate,
        daysOfWeek,
        startTime,
        endTime,
        slotDurationMinutes,
      });
      toast.notify(`${result.created} slot${result.created === 1 ? "" : "s"} created${result.skipped > 0 ? `, ${result.skipped} skipped (already booked)` : ""}.`);
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <PageHeader
        icon={RepeatIcon}
        title="Generate Recurring Slots"
        description="Create open appointment slots from a weekly schedule instead of adding them one at a time."
      />
      <Card>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Start date"
              type="date"
              required
              value={startDate}
              error={fields.startdate}
              onChange={(e) => setStartDate(e.target.value)}
            />
            <Input
              label="End date"
              type="date"
              required
              value={endDate}
              error={fields.enddate}
              onChange={(e) => setEndDate(e.target.value)}
            />
          </div>

          <div>
            <p className="mb-1.5 text-sm font-medium text-(--text-h)">Days of week</p>
            <div className="flex flex-wrap gap-1.5">
              {DAYS.map((day) => (
                <button
                  key={day}
                  type="button"
                  onClick={() => toggleDay(day)}
                  className={`rounded-(--radius) border px-3 py-1.5 text-sm font-medium transition-colors ${
                    daysOfWeek.includes(day)
                      ? "border-(--accent) bg-(--accent) text-white"
                      : "border-(--border) bg-(--surface) text-(--text) hover:bg-(--accent-bg)"
                  }`}
                >
                  {day.slice(0, 3)}
                </button>
              ))}
            </div>
            {fields.daysofweek && <p className="mt-1 text-xs text-red-600">{fields.daysofweek}</p>}
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Input
              label="Start time"
              type="time"
              required
              value={startTime}
              error={fields.starttime}
              onChange={(e) => setStartTime(e.target.value)}
            />
            <Input
              label="End time"
              type="time"
              required
              value={endTime}
              error={fields.endtime}
              onChange={(e) => setEndTime(e.target.value)}
            />
            <Input
              label="Slot duration (min)"
              type="number"
              required
              min={1}
              max={480}
              value={slotDurationMinutes}
              error={fields.slotdurationminutes}
              onChange={(e) => setSlotDurationMinutes(Number(e.target.value))}
            />
          </div>

          {errors.length > 0 && <ErrorList errors={errors} />}

          <Button type="submit" disabled={submitting} className="w-full">
            {submitting ? "Generating…" : "Generate slots"}
          </Button>
        </form>
      </Card>
    </div>
  );
}
