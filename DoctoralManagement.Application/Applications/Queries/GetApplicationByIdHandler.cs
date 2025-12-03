using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, GetApplicationByIdResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public GetApplicationByIdHandler(IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<GetApplicationByIdResponse> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new DoctoralManagementException($"Application with ID {request.Id} not found.", HttpStatusCode.NotFound);
            }

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == application.StudentId;
            bool isPrivileged = currentUserRole is "Mentor" or "Committee" or "Secretary" or "Admin";

            if (!isOwner && !isPrivileged)
            {
                throw new DoctoralManagementException("You are not authorized to view this application", HttpStatusCode.Forbidden);
            }

            return new GetApplicationByIdResponse
            {
                Id = application.Id,
                StudentId = application.StudentId,
                StudentName = application.Student.FullName,
                StudentEmail = application.Student.Email,
                DoctoralProgramId = application.DoctoralProgramId,
                ProgramName = application.DoctoralProgram.Name,
                ScientificArea = application.DoctoralProgram.ScientificArea,
                PreferredMentorId = application.PrefferedMentorId,
                PreferredMentorName = application.PrefferedMentor?.FullName,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                MeetsGradeRequirements = application.MeetsGradeRequirements,
                HasRequiredPublications = application.HasRequiredPublications,
                Documents = application.Documents.Select(d => new Dtos.ApplicationDocumentDto
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    ContentType = d.ContentType,
                    UploadedAt = d.UploadedAt
                }).ToList()
            };
        }
    }
}
