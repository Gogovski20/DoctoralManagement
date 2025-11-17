using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class CompleteThesisDefenseCommand : IRequest<CompleteThesisDefenseResponse>
    {
        public int DefenseId { get; set; }
        public string? ResultNotes { get; set; }
        public string? ArchiveNumber { get; set; } // optional
    }
}
