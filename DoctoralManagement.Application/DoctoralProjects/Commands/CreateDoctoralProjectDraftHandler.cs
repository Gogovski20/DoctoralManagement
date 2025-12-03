using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CreateDoctoralProjectDraftHandler : IRequestHandler<CreateDoctoralProjectDraftCommand, CreateDoctoralProjectDraftResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<CreateDoctoralProjectDraftHandler> _logger;

        public CreateDoctoralProjectDraftHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IMentorRepository mentorRepository, IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<CreateDoctoralProjectDraftHandler> logger)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _mentorRepository = mentorRepository;
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<CreateDoctoralProjectDraftResponse> Handle(CreateDoctoralProjectDraftCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var isAdmin = currentUserRole == "Admin";

            if (!isAdmin && (linkedStudentId == null || linkedStudentId != request.StudentId))
            {
                throw new DoctoralManagementException(
                    "You can only create a doctoral project for your own account.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found", HttpStatusCode.NotFound);

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(request.StudentId);
            if (!hasAccepted)
            {
                throw new DoctoralManagementException("Student must have FinalAccepted application to create doctoral project", HttpStatusCode.BadRequest);
            }

            var mentor = await _mentorRepository.GetByIdAsync(request.MentorId)
                ?? throw new DoctoralManagementException($"Mentor with id {request.MentorId} not found", HttpStatusCode.NotFound);

            var mentorAvailable = await _mentorRepository.IsAvailableForNewStudentAsync(request.MentorId);
            if (!mentorAvailable)
            {
                throw new DoctoralManagementException("Mentor cannot be assigned - reached maximum number of supervised students", HttpStatusCode.BadRequest);
            }

            var project = new DoctoralProject
            {
                Title = request.Title,
                ResearchArea = request.ResearchArea,
                StudentId = request.StudentId,
                MentorId = request.MentorId,
                Status = ProjectStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _doctoralProjectRepository.AddAsync(project);

            _logger.LogInformation(
                "Doctoral project draft created - Id: {ProjectId}, StudentId: {StudentId}, MentorId: {MentorId}, Title: {Title}",
                created.Id, created.StudentId, created.MentorId, created.Title);

            return new CreateDoctoralProjectDraftResponse
            {
                Id = created.Id,
                Title = created.Title,
                ResearchArea = created.ResearchArea,
                Status = created.Status.ToString(),
                CreatedAt = created.CreatedAt
            };
        }
    }
}
