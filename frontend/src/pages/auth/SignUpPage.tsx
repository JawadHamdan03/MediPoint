import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { Textarea } from "../../components/ui/Textarea";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError, fieldErrors } from "../../lib/formatError";
import { ProfileIcon, StethoscopeIcon } from "../../components/ui/icons";
import type { DoctorSignUpDto, PatientSignUpDto, SignUpRole } from "../../types/auth";

const ROLES: { role: SignUpRole; icon: typeof ProfileIcon }[] = [
  { role: "Patient", icon: ProfileIcon },
  { role: "Doctor", icon: StethoscopeIcon },
];

const initialPatient: PatientSignUpDto = {
  firstName: "",
  lastName: "",
  password: "",
  email: "",
  phoneNumber: "",
  dateOfBirth: "",
  gender: "Male",
  bloodType: "",
  address: "",
  emergencyContactName: "",
  emergencyContactPhone: "",
};

const initialDoctor: DoctorSignUpDto = {
  firstName: "",
  lastName: "",
  password: "",
  email: "",
  phoneNumber: "",
  dateOfBirth: "",
  gender: "Male",
  specialty: "",
  licenseNumber: "",
  yearsOfExperience: 0,
  consultationFee: 0,
  biography: "",
};

export default function SignUpPage() {
  const auth = useAuth();
  const navigate = useNavigate();
  const [role, setRole] = useState<SignUpRole>("Patient");
  const [patient, setPatient] = useState<PatientSignUpDto>(initialPatient);
  const [doctor, setDoctor] = useState<DoctorSignUpDto>(initialDoctor);
  const [errors, setErrors] = useState<string[]>([]);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  function updatePatient<K extends keyof PatientSignUpDto>(key: K, value: PatientSignUpDto[K]) {
    setPatient((f) => ({ ...f, [key]: value }));
  }

  function updateDoctor<K extends keyof DoctorSignUpDto>(key: K, value: DoctorSignUpDto[K]) {
    setDoctor((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setFields({});
    setSubmitting(true);
    try {
      await auth.signUp(role, role === "Patient" ? patient : doctor);
      navigate(`/${role.toLowerCase()}`, { replace: true });
    } catch (err) {
      setErrors(formatError(err));
      setFields(fieldErrors(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div
      className="relative flex min-h-screen items-center justify-center overflow-hidden bg-(--bg) px-4 py-10"
      style={{ animation: "fade-in 0.2s ease-out" }}
    >
      <div
        aria-hidden
        className="pointer-events-none absolute -top-32 left-1/2 h-80 w-[36rem] -translate-x-1/2 rounded-full bg-(--accent) opacity-[0.12] blur-3xl"
      />
      <div className="relative w-full max-w-2xl">
        <div className="mb-6 flex flex-col items-center gap-2 text-center">
          <span className="flex h-11 w-11 items-center justify-center rounded-(--radius) bg-(--accent) text-lg font-bold text-white shadow-(--shadow)">
            M
          </span>
          <h1 className="text-2xl font-semibold text-(--text-h)">MediPoint</h1>
          <p className="text-sm text-(--text)">Create an account to get started</p>
        </div>

        <Card>
          <div className="mb-5 flex rounded-(--radius) border border-(--border) bg-(--bg) p-1">
            {ROLES.map(({ role: r, icon: Icon }) => (
              <button
                key={r}
                type="button"
                onClick={() => setRole(r)}
                className={`flex flex-1 items-center justify-center gap-1.5 rounded-[calc(var(--radius)-4px)] px-3 py-1.5 text-sm font-medium transition-colors duration-150 ${
                  role === r ? "bg-(--accent) text-white shadow-sm" : "text-(--text) hover:bg-(--accent-bg)"
                }`}
              >
                <Icon className="h-4 w-4" />
                {r}
              </button>
            ))}
          </div>

          {role === "Patient" ? (
            <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <Input
                label="First name"
                required
                maxLength={50}
                value={patient.firstName}
                error={fields.firstname}
                onChange={(e) => updatePatient("firstName", e.target.value)}
              />
              <Input
                label="Last name"
                required
                maxLength={50}
                value={patient.lastName}
                error={fields.lastname}
                onChange={(e) => updatePatient("lastName", e.target.value)}
              />
              <Input
                label="Email"
                type="email"
                autoComplete="email"
                required
                maxLength={100}
                value={patient.email}
                error={fields.email}
                onChange={(e) => updatePatient("email", e.target.value)}
              />
              <Input
                label="Password"
                type="password"
                autoComplete="new-password"
                required
                minLength={8}
                value={patient.password}
                error={fields.password}
                onChange={(e) => updatePatient("password", e.target.value)}
              />
              <Input
                label="Phone number"
                required
                placeholder="+15551234567"
                pattern="^\+?[1-9]\d{1,14}$"
                value={patient.phoneNumber}
                error={fields.phonenumber}
                onChange={(e) => updatePatient("phoneNumber", e.target.value)}
              />
              <Input
                label="Date of birth"
                type="date"
                required
                value={patient.dateOfBirth}
                error={fields.dateofbirth}
                onChange={(e) => updatePatient("dateOfBirth", e.target.value)}
              />
              <Select
                label="Gender"
                value={patient.gender}
                onChange={(e) => updatePatient("gender", e.target.value as PatientSignUpDto["gender"])}
              >
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </Select>
              <Input
                label="Blood type"
                maxLength={10}
                value={patient.bloodType}
                error={fields.bloodtype}
                onChange={(e) => updatePatient("bloodType", e.target.value)}
              />
              <div className="sm:col-span-2">
                <Input
                  label="Address"
                  maxLength={200}
                  value={patient.address}
                  error={fields.address}
                  onChange={(e) => updatePatient("address", e.target.value)}
                />
              </div>
              <Input
                label="Emergency contact name"
                maxLength={100}
                value={patient.emergencyContactName}
                error={fields.emergencycontactname}
                onChange={(e) => updatePatient("emergencyContactName", e.target.value)}
              />
              <Input
                label="Emergency contact phone"
                maxLength={20}
                value={patient.emergencyContactPhone}
                error={fields.emergencycontactphone}
                onChange={(e) => updatePatient("emergencyContactPhone", e.target.value)}
              />

              {errors.length > 0 && (
                <div className="sm:col-span-2">
                  <ErrorList errors={errors} />
                </div>
              )}

              <div className="sm:col-span-2">
                <Button type="submit" disabled={submitting} className="w-full">
                  {submitting ? "Creating account…" : "Create patient account"}
                </Button>
              </div>
            </form>
          ) : (
            <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <Input
                label="First name"
                required
                maxLength={50}
                value={doctor.firstName}
                error={fields.firstname}
                onChange={(e) => updateDoctor("firstName", e.target.value)}
              />
              <Input
                label="Last name"
                required
                maxLength={50}
                value={doctor.lastName}
                error={fields.lastname}
                onChange={(e) => updateDoctor("lastName", e.target.value)}
              />
              <Input
                label="Email"
                type="email"
                autoComplete="email"
                required
                maxLength={100}
                value={doctor.email}
                error={fields.email}
                onChange={(e) => updateDoctor("email", e.target.value)}
              />
              <Input
                label="Password"
                type="password"
                autoComplete="new-password"
                required
                minLength={8}
                value={doctor.password}
                error={fields.password}
                onChange={(e) => updateDoctor("password", e.target.value)}
              />
              <Input
                label="Phone number"
                required
                placeholder="+15551234567"
                pattern="^\+?[1-9]\d{1,14}$"
                value={doctor.phoneNumber}
                error={fields.phonenumber}
                onChange={(e) => updateDoctor("phoneNumber", e.target.value)}
              />
              <Input
                label="Date of birth"
                type="date"
                required
                value={doctor.dateOfBirth}
                error={fields.dateofbirth}
                onChange={(e) => updateDoctor("dateOfBirth", e.target.value)}
              />
              <Select
                label="Gender"
                value={doctor.gender}
                onChange={(e) => updateDoctor("gender", e.target.value as DoctorSignUpDto["gender"])}
              >
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </Select>
              <Input
                label="Specialty"
                required
                maxLength={100}
                value={doctor.specialty}
                error={fields.specialty}
                onChange={(e) => updateDoctor("specialty", e.target.value)}
              />
              <Input
                label="License number"
                required
                maxLength={50}
                value={doctor.licenseNumber}
                error={fields.licensenumber}
                onChange={(e) => updateDoctor("licenseNumber", e.target.value)}
              />
              <Input
                label="Years of experience"
                type="number"
                min={0}
                max={70}
                value={doctor.yearsOfExperience}
                error={fields.yearsofexperience}
                onChange={(e) => updateDoctor("yearsOfExperience", Number(e.target.value))}
              />
              <Input
                label="Consultation fee"
                type="number"
                min={0}
                step="0.01"
                value={doctor.consultationFee}
                error={fields.consultationfee}
                onChange={(e) => updateDoctor("consultationFee", Number(e.target.value))}
              />
              <div className="sm:col-span-2">
                <Textarea
                  label="Biography"
                  maxLength={1000}
                  rows={3}
                  value={doctor.biography}
                  error={fields.biography}
                  onChange={(e) => updateDoctor("biography", e.target.value)}
                />
              </div>

              {errors.length > 0 && (
                <div className="sm:col-span-2">
                  <ErrorList errors={errors} />
                </div>
              )}

              <div className="sm:col-span-2">
                <Button type="submit" disabled={submitting} className="w-full">
                  {submitting ? "Creating account…" : "Create doctor account"}
                </Button>
              </div>
            </form>
          )}
        </Card>

        <p className="mt-4 text-center text-sm text-(--text)">
          Already have an account?{" "}
          <Link to="/login" className="font-medium text-(--accent)">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
