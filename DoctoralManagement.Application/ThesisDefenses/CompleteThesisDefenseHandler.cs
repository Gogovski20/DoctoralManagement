using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class CompleteThesisDefenseHandler : IRequestHandler<CompleteThesisDefenseCommand, CompleteThesisDefenseResponse>
    {
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IStudentRepository _studentRepository;

        public CompleteThesisDefenseHandler(IThesisDefenseRepository thesisDefenseRepository, IDoctoralProjectRepository projectRepository, IStudentRepository studentRepository)
        {
            _thesisDefenseRepository = thesisDefenseRepository;
            _projectRepository = projectRepository;
            _studentRepository = studentRepository;
        }

        public async Task<CompleteThesisDefenseResponse> Handle(CompleteThesisDefenseCommand request, CancellationToken cancellationToken)
        {
            var defense = await _thesisDefenseRepository.GetByIdAsync(request.DefenseId)
                ?? throw new Exception($"Defense with id {request.DefenseId} not found");

            if (defense.Status != DefenseStatus.Scheduled)
                throw new Exception("Only scheduled defenses can be completed.");

            if (DateTime.UtcNow < defense.ScheduledAt.AddMinutes(-5))
            {
                throw new Exception($"Defense cannot be marked complete until scheduled time ({defense.ScheduledAt}).");
            }

            // Set defense as completed (NOT passed/failed yet)
            defense.Status = DefenseStatus.Completed;
            defense.ResultNotes = request.ResultNotes;
            defense.CompletedAt = DateTime.UtcNow;

            // Generate archive number once defense is completed
            defense.ArchiveNumber = string.IsNullOrWhiteSpace(request.ArchiveNumber)
                ? GenerateArchiveNumber(defense)
                : request.ArchiveNumber;

            await _thesisDefenseRepository.UpdateAsync(defense);

            return new CompleteThesisDefenseResponse
            {
                DefenseId = defense.Id,
                ProjectId = defense.DoctoralProjectId,
                Status = defense.Status.ToString(),
                ArchiveNumber = defense.ArchiveNumber,
                CompletedAt = defense.CompletedAt
            };
        }


        private string GenerateArchiveNumber(ThesisDefense d)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return $"DR-{d.DoctoralProjectId}-{timestamp}";
        }
    }
}
