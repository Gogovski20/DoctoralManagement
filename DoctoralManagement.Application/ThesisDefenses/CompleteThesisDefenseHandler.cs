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

            if (defense.Status != Domain.Entities.DefenseStatus.Scheduled)
            {
                throw new Exception("Only scheduled defenses can be completed");
            }

            if (request.Result != DefenseStatus.Passed &&
                request.Result != DefenseStatus.Failed)
            {
                throw new Exception("Defense result must be Passed or Failed");
            }

            defense.Status = request.Result;
            defense.ResultNotes = request.ResultNotes;
            defense.CompletedAt = DateTime.UtcNow;

            if (request.Result == DefenseStatus.Passed)
            {
                defense.ArchiveNumber = string.IsNullOrWhiteSpace(request.ArchiveNumber)
                    ? GenerateArchiveNumber(defense)
                    : request.ArchiveNumber;

                var project = await _projectRepository.GetByIdAsync(defense.DoctoralProjectId)
                    ?? throw new Exception("Associated project not found");

                project.Status = ProjectStatus.Completed;
                await _projectRepository.UpdateAsync(project);

                var student = await _studentRepository.GetByIdAsync(project.StudentId)
                    ?? throw new Exception("Student not found");

                student.Status = StudentStatus.Graduated;
                await _studentRepository.UpdateAsync(student);
            }

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
            var year = DateTime.UtcNow.Year;
            var rand = new Random().Next(1000, 9999);
            return $"DEF-{year}-{d.DoctoralProjectId}-{rand}";
        }
    }
}
