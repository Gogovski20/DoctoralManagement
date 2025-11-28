using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

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
                throw new Exception($"Application with ID {request.Id} not found.");
            }

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == application.StudentId;
            bool isPrivileged = currentUserRole is "Mentor" or "Committee" or "Secretary" or "Admin";

            if (!isOwner && !isPrivileged)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this application");
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
                MotivationLetter = application.MotivationLetter,
                ResearchProposal = application.ResearchProposal,
                EnglishCertificatePath = application.EnglishCertificatePath,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                MeetsGradeRequirements = application.MeetsGradeRequirements,
                HasRequiredPublications = application.HasRequiredPublications
            };
        }
    }
}
