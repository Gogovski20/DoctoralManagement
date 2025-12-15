using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetAllThesisDefensesHandler : IRequestHandler<GetAllThesisDefensesQuery, IEnumerable<GetAllThesisDefensesResponse>>
    {
        private readonly IThesisDefenseRepository _repository;

        public GetAllThesisDefensesHandler(IThesisDefenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetAllThesisDefensesResponse>> Handle(GetAllThesisDefensesQuery request, CancellationToken cancellationToken)
        {
            var defenses = await _repository.GetAllAsync();

            return defenses.Select(defense => new GetAllThesisDefensesResponse
            {
                Id = defense.Id,
                StudentName = defense.DoctoralProject?.Student?.FullName ?? "N/A",
                ProjectTitle = defense.DoctoralProject?.Title ?? "N/A",
                ScheduledAt = defense.ScheduledAt,
                Room = defense.Room,
                CommitteeMembers = defense.CommitteeMemberIds,
                Status = defense.Status,
                ResultNotes = defense.ResultNotes,
                CompletedAt = defense.CompletedAt,
                ArchiveNumber = defense.ArchiveNumber,
                Reviews = defense.Reviews
            });
        }
    }
}
