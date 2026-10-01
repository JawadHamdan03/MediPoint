using MediatR;
using MediPoint.Application.Features.Patients.BookAppointment;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.FindDoctors;
using MediPoint.Application.Features.Patients.Login;
using MediPoint.Application.Features.Patients.RefreshPatientToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediPoint.Application.Features.Patients.GetRecords;
using MediPoint.Application.Features.Patients.CancelAppointment;
using MediPoint.Application.Features.Patients.CancelAppointment.DTOs;
using MediPoint.Application.Features.Patients.UpdateDetails;
using MediPoint.Application.Features.Patients.UpdateDetails.DTOs;
using MediPoint.Application.Features.Patients.Chat;
using MediPoint.Application.Features.Patients.Chat.DTOs;
using MediPoint.Application.Features.Patients.SignUp;
using MediPoint.Application.Features.Patients.SignUp.DTOs;
using MediPoint.Application.Features.Patients.ForgotPassword;
using MediPoint.Application.Features.Patients.ResetPassword;
using MediPoint.Application.Features.Patients.AddReview;
using MediPoint.Application.Features.Patients.AddReview.DTOs;
using MediPoint.Application.Features.Patients.GetDoctorReviews;
using MediPoint.Application.Features.Patients.GetMyAppointments;
using MediPoint.Application.Features.Users.GetPrescriptionPdf;

namespace MediPoint.Api.Controllers;


[Route("/patients")]
[ApiController]
public class PatientController(IMediator mediator) : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(PatientSignUpDto signUpRequest)
    {
        var res = await mediator.Send(new SignUpPatientCommand(signUpRequest));
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var res = await mediator.Send(new PatientLoginCommand(loginRequest));

        return Ok(res);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> forgotPassword(ForgotPasswordRequest request)
    {
        var res = await mediator.Send(new PatientForgotPasswordCommand(request));
        return Ok(res);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> resetPassword(ResetPasswordRequest request)
    {
        var res = await mediator.Send(new PatientResetPasswordCommand(request));
        return Ok(res);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> refreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        var res = await mediator.Send(new RefreshPatienttokenRequest(refreshTokenRequest.RefreshToken));
        return Ok(res);
    }

    [Authorize(Roles ="Patient")]
    [HttpGet("search-doctors/{speciality}")]
    public async Task<IActionResult> findDoctors(string speciality)
    {
        var res = await mediator.Send(new FindDoctorsQuery(speciality));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("book-appointment/{appointmentId}")]
    public async Task<IActionResult> bookAppointment(Guid appointmentId)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier);
        var res = await mediator.Send(new BookAppointmentCommand(
                new BookAppointmentRequest
                {
                    AppointmentId = appointmentId,
                    PatientId = Guid.Parse(patientId.Value)
                }));

        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("get-medical-records")]
    public async Task<IActionResult> GetMedicalRecords()
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new GetRecordsCommand(Guid.Parse(patientId!)));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("cancel-appointment/{appointmentId}")]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId, CancelAppointmentRequest request)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new CancelAppointmentCommand(appointmentId, Guid.Parse(patientId!), request.CancellationReason));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("update-details")]
    public async Task<IActionResult> UpdateDetails(UpdatePatientDto details)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new UpdatePatientDetailsCommand(Guid.Parse(patientId!), details));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("appointments")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new GetMyAppointmentsQuery(Guid.Parse(patientId!)));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("appointments/{appointmentId}/review")]
    public async Task<IActionResult> AddReview(Guid appointmentId, AddReviewRequest request)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new AddReviewCommand(appointmentId, Guid.Parse(patientId!), request));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("doctors/{doctorId}/reviews")]
    public async Task<IActionResult> GetDoctorReviews(Guid doctorId)
    {
        var res = await mediator.Send(new GetDoctorReviewsQuery(doctorId));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("prescriptions/{prescriptionId}/pdf")]
    public async Task<IActionResult> GetPrescriptionPdf(Guid prescriptionId)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var pdf = await mediator.Send(new GetPrescriptionPdfQuery(prescriptionId, Guid.Parse(patientId!), "Patient"));
        return File(pdf, "application/pdf", "prescription.pdf");
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(ChatRequest request)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new ChatCommand(request.Message, Guid.Parse(patientId!)));
        return Ok(res);
    }


}
