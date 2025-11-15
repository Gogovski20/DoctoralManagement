using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class DeleteMentorHandler : IRequestHandler<DeleteMentorCommand, bool>
    {
        private readonly IMentorRepository _mentorRepository;

        public DeleteMentorHandler(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public async Task<bool> Handle(DeleteMentorCommand request, CancellationToken cancellationToken)
        {
            await _mentorRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}
