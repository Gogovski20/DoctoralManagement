using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetScheduledDefensesHandler : IRequestHandler<GetScheduledDefensesQuery, IEnumerable<ScheduledDefenseResponse>>
    {
        private readonly IThesisDefenseRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetScheduledDefensesHandler> _logger;

        public GetScheduledDefensesHandler(IThesisDefenseRepository repository, ICurrentUserService currentUserService, ILogger<GetScheduledDefensesHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<ScheduledDefenseResponse>> Handle(GetScheduledDefensesQuery request, CancellationToken cancellationToken)
        {
            var defenses = await _repository.GetByStatusAsync(Domain.Entities.DefenseStatus.Scheduled);

            if (defenses == null || !defenses.Any())
            {
                return Enumerable.Empty<ScheduledDefenseResponse>();
            }

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            _logger.LogInformation(
                "{Role} {UserId} retrieved scheduled defenses. Count: {Count}",
                currentUserRole, currentUserId, defenses.Count());

            return defenses.Select(d => new ScheduledDefenseResponse
            {
                Id = d.Id,
                ProjectId = d.DoctoralProjectId,
                StudentName = d.DoctoralProject?.Student?.FullName,
                ScheduledAt = d.ScheduledAt,
                Room = d.Room,
                CommitteeMemberCount = d.CommitteeMemberIds?.Count ?? 0,
                Status = d.Status.ToString()
            }).ToList();
        }
    }
}
