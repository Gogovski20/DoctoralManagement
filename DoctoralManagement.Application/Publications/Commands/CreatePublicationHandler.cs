using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class CreatePublicationHandler : IRequestHandler<CreatePublicationCommand, CreatePublicationResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;

        public CreatePublicationHandler(
            IStudentRepository studentRepository,
            IPublicationRepository publicationRepository,
            IEctsTrackingRepository ectsRepository,
            IApplicationRepository applicationRepository,
            EctsProgressService ectsProgressService)
        {
            _studentRepository = studentRepository;
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
        }
        public async Task<CreatePublicationResponse> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);

            if (!hasAccepted)
            {
                throw new Exception("Student is not accepted to a doctoral program");
            }

            int ectsPoints = CalculateEctsForPublication(request.IsIndexedInScopus, request.IsIndexedInThomsonReuters);

            var publication = new Publication
            {
                StudentId = request.StudentId,
                Title = request.Title,
                Journal = request.Journal,
                PublishedOn = request.PublishedOn,
                Doi = request.Doi,
                IsIndexedInScopus = request.IsIndexedInScopus,
                IsIndexedInThomsonReuters = request.IsIndexedInThomsonReuters,
                EctsPoints = ectsPoints
            };

            var created = await _publicationRepository.AddAsync(publication);

            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(request.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.Publications += ectsPoints;
                if (ectsTracking.Publications > 27)
                {
                    ectsTracking.Publications = 27;
                }
                await _ectsRepository.UpdateAsync(ectsTracking);
                await _ectsProgressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
            }

            return new CreatePublicationResponse
            {
                Id = created.Id,
                StudentId = created.StudentId,
                Title = created.Title,
                EctsAwarded = ectsPoints
            };
        }

        private int CalculateEctsForPublication(bool isScopus, bool isThomson)
        {
            if (isScopus || isThomson)
                return 7; // High-quality publication
            return 3; // others
        }
    }
}
