using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, GetApplicationByIdResponse>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationByIdHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<GetApplicationByIdResponse> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new Exception($"Application with ID {request.Id} not found.");
            }

            return new GetApplicationByIdResponse
            {
                Id = application.Id,
                StudentId = application.StudentId,
                StudentName = application.Student.FullName,
                StudentEmail = application.Student.Email,
                DoctoralProgramId = application.DoctoralProgramId,
                ProgramName = application.DoctoralProgram.Name,
                ScientificArea = application.DoctoralProgram.ScientificArea,
                PreferredMentorId = application.PrefferedMentorId,
                PreferredMentorName = application.PrefferedMentor?.FullName,
                MotivationLetter = application.MotivationLetter,
                ResearchProposal = application.ResearchProposal,
                EnglishCertificatePath = application.EnglishCertificatePath,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                MeetsGradeRequirements = application.MeetsGradeRequirements,
                HasRequiredPublications = application.HasRequiredPublications
            };
        }
    }
}
