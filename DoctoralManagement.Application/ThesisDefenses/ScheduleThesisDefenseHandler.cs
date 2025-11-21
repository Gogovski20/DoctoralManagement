using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ScheduleThesisDefenseHandler : IRequestHandler<ScheduleThesisDefenseCommand, ScheduleThesisDefenseResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IStudentRepository _studentRepository;

        public ScheduleThesisDefenseHandler(IDoctoralProjectRepository projectRepository, IThesisDefenseRepository thesisDefenseRepository, IMentorRepository mentorRepository, IEctsTrackingRepository ectsTrackingRepository, IStudentRepository studentRepository)
        {
            _projectRepository = projectRepository;
            _thesisDefenseRepository = thesisDefenseRepository;
            _mentorRepository = mentorRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _studentRepository = studentRepository;
        }

        public async Task<ScheduleThesisDefenseResponse> Handle(ScheduleThesisDefenseCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new Exception("Project not found");

            if (project.Status != ProjectStatus.Approved)
            {
                throw new Exception("Project must be approved before scheduling defense");
            }

            if (await _thesisDefenseRepository.ExistsForProjectAsync(request.ProjectId))
            {
                throw new Exception("A defense is already scheduled for this project");
            }

            var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(project.StudentId)
                ?? throw new Exception("ECTS tracking not found for student.");
            int currentEcts = ectsTracking.TotalECTS;
            if (currentEcts < 134)
            {
                throw new Exception($"Student must have at least 134 ECTS before defense scheduling. Current: {currentEcts} ECTS.");
            }

            var student = await _studentRepository.GetByIdAsync(project.StudentId);
            if (student?.CurrentSemester < 5)
            {
                throw new Exception("Student must be in semester 5 or later to schedule defense (typically final semester).");
            }


            if (request.CommitteeMemberIds == null || request.CommitteeMemberIds.Count < 3)
            {
                throw new Exception("At least 3 committee members are required");
            }

            if (!request.CommitteeMemberIds.Contains(project.MentorId))
            {
                throw new Exception("Project mentor must be part of the defense committee.");
            }

            foreach (var memberId in request.CommitteeMemberIds)
            {
                var mentor = await _mentorRepository.GetByIdAsync(memberId);
                if (mentor == null)
                {
                    throw new Exception($"Committee member {memberId} not found");
                }
            }

            if (request.ScheduledAt <= DateTime.UtcNow)
            {
                throw new Exception("Defense must be scheduled in the future");
            }

            if (string.IsNullOrWhiteSpace(request.Room))
            {
                throw new Exception("Room is required");
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
