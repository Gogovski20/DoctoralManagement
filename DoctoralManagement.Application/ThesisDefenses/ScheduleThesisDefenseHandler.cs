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

        public ScheduleThesisDefenseHandler(IDoctoralProjectRepository projectRepository, IThesisDefenseRepository thesisDefenseRepository, IMentorRepository mentorRepository)
        {
            _projectRepository = projectRepository;
            _thesisDefenseRepository = thesisDefenseRepository;
            _mentorRepository = mentorRepository;
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

            if (request.CommitteeMemberIds == null || request.CommitteeMemberIds.Count < 3)
            {
                throw new Exception("At least 3 committee members are required");
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
