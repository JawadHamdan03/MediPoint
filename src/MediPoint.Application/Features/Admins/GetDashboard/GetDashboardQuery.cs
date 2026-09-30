using MediatR;
using MediPoint.Application.Features.Admins.GetDashboard.DTOs;

namespace MediPoint.Application.Features.Admins.GetDashboard;

public record GetDashboardQuery : IRequest<AdminDashboardResponse>;
