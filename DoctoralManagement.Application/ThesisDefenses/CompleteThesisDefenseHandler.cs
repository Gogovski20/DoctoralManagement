using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class CompleteThesisDefenseHandler : IRequestHandler<CompleteThesisDefenseCommand, CompleteThesisDefenseResponse>
    {
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CompleteThesisDefenseHandler> _logger;

        public CompleteThesisDefenseHandler(IThesisDefenseRepository thesisDefenseRepository, IDoctoralProjectRepository projectRepository, IStudentRepository studentRepository, ICurrentUserService currentUserService, ILogger<CompleteThesisDefenseHandler> logger)
        {
            _thesisDefenseRepository = thesisDefenseRepository;
            _projectRepository = projectRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<CompleteThesisDefenseResponse> Handle(CompleteThesisDefenseCommand request, CancellationToken cancellationToken)
        {
            var currentUserRole = _currentUserService.Role;
            if (currentUserRole != "Admin")
            {
                throw new DoctoralManagementException(
                    "Only admins can mark defense as complete.",
                    HttpStatusCode.Forbidden);
            }

            var defense = await _thesisDefenseRepository.GetByIdAsync(request.DefenseId)
                ?? throw new DoctoralManagementException($"Defense with id {request.DefenseId} not found", HttpStatusCode.NotFound);

            if (defense.Status != DefenseStatus.Scheduled)
                throw new DoctoralManagementException("Only scheduled defenses can be completed.", HttpStatusCode.BadRequest);

            if (DateTime.UtcNow < defense.ScheduledAt.AddMinutes(-5))
            {
                throw new DoctoralManagementException($"Defense cannot be marked complete until scheduled time ({defense.ScheduledAt}).", HttpStatusCode.BadRequest);
            }

            defense.Status = DefenseStatus.Completed;
            defense.ResultNotes = request.ResultNotes;
            defense.CompletedAt = DateTime.UtcNow;

            defense.ArchiveNumber = string.IsNullOrWhiteSpace(request.ArchiveNumber)
                ? GenerateArchiveNumber(defense)
                : request.ArchiveNumber;

            await _thesisDefenseRepository.UpdateAsync(defense);

            _logger.LogInformation(
                "Thesis defense {DefenseId} marked as complete. Archive number: {ArchiveNumber}",
                defense.Id, defense.ArchiveNumber);

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
