using System.ComponentModel;
using MediatR;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.FindDoctors;
using MediPoint.Application.Features.Patients.GetRecords;
using MediPoint.Application.Features.Patients.GetRecords.DTOs;

namespace MediPoint.Application.Features.Patients.Chat;

/// <summary>
/// The tools the patient assistant is allowed to call. Each one maps to an existing MediatR feature,
/// so its validation, ownership checks, and exception→HTTP mapping are all reused for free.
/// The patient id is captured here from the JWT-derived claim (supplied by the handler) — the model
/// never provides it, so the agent can only ever act on the signed-in patient's own data.
/// </summary>
internal sealed class PatientChatTools(ISender sender, Guid patientId, CancellationToken cancellationToken)
{
    [Description("Find available doctors by medical specialty (e.g. 'Cardiology', 'Dermatology'). " +
                 "Returns each matching doctor with their profile and open appointment slots.")]
    public Task<List<DoctorResponse>> FindDoctorsAsync(
        [Description("The medical specialty to search for, e.g. 'Cardiology'.")] string speciality)
        => sender.Send(new FindDoctorsQuery(speciality), cancellationToken);

    [Description("Get the signed-in patient's own medical records: prescriptions, diagnoses, " +
                 "medicines, and lab results. Takes no arguments — the patient is already identified.")]
    public Task<List<MedicalRecordResponse>> GetMyMedicalRecordsAsync()
        => sender.Send(new GetRecordsCommand(patientId), cancellationToken);
}
