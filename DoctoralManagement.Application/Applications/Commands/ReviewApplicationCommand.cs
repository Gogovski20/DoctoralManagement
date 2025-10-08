using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class ReviewApplicationCommand : IRequest<ReviewApplicationResponse>
    {
        public int Id { get; set; }
        public ApplicationStatus NewStatus { get; set; }
        public string ReviewComments { get; set; } = string.Empty;
        public bool HasRequiredPublications { get; set; }
    }
}
