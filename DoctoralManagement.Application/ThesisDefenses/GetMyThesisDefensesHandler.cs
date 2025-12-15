using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetMyThesisDefensesHandler : IRequestHandler<GetMyThesisDefensesQuery, IEnumerable<GetMyThesisDefensesResponse>>
    {
        private readonly IThesisDefenseRepository _repository;

        public GetMyThesisDefensesHandler(IThesisDefenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetMyThesisDefensesResponse>> Handle(GetMyThesisDefensesQuery request, CancellationToken cancellationToken)
        {
            var defenses = await _repository.GetStudentDefenses(request.StudentId);

            return defenses.Select(defense => new GetMyThesisDefensesResponse
            {
                Id = defense.Id,
                ProjectId = defense.DoctoralProjectId,
                ProjectTitle = defense.DoctoralProject.Title,
                ScheduledAt = defense.ScheduledAt,
                Room = defense.Room,
                CommitteeMembers = defense.CommitteeMemberIds,
                Status = defense.Status.ToString()
            });
        }
    }
}
