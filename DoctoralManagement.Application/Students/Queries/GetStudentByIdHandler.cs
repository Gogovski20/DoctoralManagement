using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, GetStudentByIdResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetStudentByIdHandler> _logger;

        public GetStudentByIdHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetStudentByIdHandler> logger)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetStudentByIdResponse> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var isAdmin = currentUserRole == "Admin";

            if (!isAdmin && (linkedStudentId == null || linkedStudentId != request.Id))
            {
                throw new DoctoralManagementException(
                    "You can only view your own student profile.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdWithProgramNoTrackingAsync(request.Id);

            if (student == null)
            {
                throw new DoctoralManagementException($"Student with ID {request.Id} not found.", HttpStatusCode.NotFound);
            }

            _logger.LogInformation(
               "{Role} {UserId} viewed student profile for student {StudentId}",
               currentUserRole, currentUserId, request.Id);

            return new GetStudentByIdResponse
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                IndexNumber = student.IndexNumber,
                EnrollmentDate = student.EnrollmentDate,
                GPA = student.GPA,
                EnglishCertificate = student.EnglishCertificate,
                StudentStatus = student.Status,
                TotalCreditsFromBachelor = student.TotalCreditsFromBachelor,
                TotalCreditsFromMaster = student.TotalCreditsFromMaster,
                TotalCredits = student.TotalCredits,
                DoctoralProgramId = student.DoctoralProgramId,
                DoctoralProgramName = student.DoctoralProgram?.Name
            };
        }
    }
}
