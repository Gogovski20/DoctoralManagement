using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ScheduleThesisDefenseHandler : IRequestHandler<ScheduleThesisDefenseCommand, ScheduleThesisDefenseResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ScheduleThesisDefenseHandler> _logger;

        public ScheduleThesisDefenseHandler(IDoctoralProjectRepository projectRepository, IThesisDefenseRepository thesisDefenseRepository, IMentorRepository mentorRepository, IEctsTrackingRepository ectsTrackingRepository, IStudentRepository studentRepository, ICurrentUserService currentUserService, ILogger<ScheduleThesisDefenseHandler> logger)
        {
            _projectRepository = projectRepository;
            _thesisDefenseRepository = thesisDefenseRepository;
            _mentorRepository = mentorRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ScheduleThesisDefenseResponse> Handle(ScheduleThesisDefenseCommand request, CancellationToken cancellationToken)
        {
            var currentUserRole = _currentUserService.Role;
            if (currentUserRole != "Admin")
            {
                throw new DoctoralManagementException(
                    "Only admins can schedule thesis defenses.",
                    HttpStatusCode.Forbidden);
            }

            var project = await _projectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new DoctoralManagementException("Project not found", HttpStatusCode.NotFound);

            if (project.Status != ProjectStatus.DefenseUnderReview)
            {
                throw new DoctoralManagementException("Thesis document must be reviewed and approved before scheduling defense.", HttpStatusCode.BadRequest);
            }

            if (await _thesisDefenseRepository.ExistsForProjectAsync(request.ProjectId))
            {
                throw new DoctoralManagementException("A defense is already scheduled for this project", HttpStatusCode.BadRequest);
            }

            var thesisDoc = project.Documents?.FirstOrDefault(d =>
                d.DocumentType == ActivityDocumentType.DefenseThesisDocument &&
                d.Status == DocumentStatus.Approved);
    
            if (thesisDoc == null)
            {
                throw new DoctoralManagementException("Approved thesis document is required before scheduling defense.", HttpStatusCode.NotFound);
            }



            var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(project.StudentId)
                ?? throw new DoctoralManagementException("ECTS tracking not found for student.", HttpStatusCode.NotFound);

            if (ectsTracking.ThesisDefence < 20)
            {
                throw new DoctoralManagementException($"Student must earn first 20 Thesis ECTS before defense scheduling. Current: {ectsTracking.ThesisDefence} ECTS.", HttpStatusCode.BadRequest);
            }

            int currentEcts = ectsTracking.TotalECTS;
            if (currentEcts < 134)
            {
                throw new DoctoralManagementException($"Student must have at least 134 ECTS before defense scheduling. Current: {currentEcts} ECTS.", HttpStatusCode.BadRequest);
            }

            var student = await _studentRepository.GetByIdAsync(project.StudentId);
            if (student?.CurrentSemester < 5)
            {
                throw new DoctoralManagementException("Student must be in semester 5 or later to schedule defense (typically final semester).", HttpStatusCode.BadRequest);
            }


            if (request.CommitteeMemberIds == null || request.CommitteeMemberIds.Count < 3)
            {
                throw new DoctoralManagementException("At least 3 committee members are required", HttpStatusCode.BadRequest);
            }

            if (!request.CommitteeMemberIds.Contains(project.MentorId))
            {
                throw new DoctoralManagementException("Project mentor must be part of the defense committee.", HttpStatusCode.BadRequest);
            }

            foreach (var memberId in request.CommitteeMemberIds)
            {
                var mentor = await _mentorRepository.GetByIdAsync(memberId);
                if (mentor == null)
                {
                    throw new DoctoralManagementException($"Committee member {memberId} not found", HttpStatusCode.NotFound);
                }
            }

            if (request.ScheduledAt <= DateTime.UtcNow)
            {
                throw new DoctoralManagementException("Defense must be scheduled in the future", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Room))
            {
                throw new DoctoralManagementException("Room is required", HttpStatusCode.BadRequest);
            }

            var defense = new ThesisDefense
            {
                DoctoralProjectId = request.ProjectId,
                ScheduledAt = request.ScheduledAt,
                Room = request.Room,
                CommitteeMemberIds = request.CommitteeMemberIds,
                Status = DefenseStatus.Scheduled
            };

            var created = await _thesisDefenseRepository.AddAsync(defense);

            _logger.LogInformation(
                "Thesis defense scheduled for project {ProjectId}. DefenseId: {DefenseId}, ScheduledAt: {ScheduledAt}",
                request.ProjectId, created.Id, created.ScheduledAt);

            return new ScheduleThesisDefenseResponse
            {
                Id = created.Id,
                ProjectId = created.DoctoralProjectId,
                ScheduledAt = created.ScheduledAt,
                Room = created.Room,
                CommitteeMemberIds = created.CommitteeMemberIds,
                Status = created.Status.ToString()
            };
        }
    }
}
