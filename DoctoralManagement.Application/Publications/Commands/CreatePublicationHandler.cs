using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class CreatePublicationHandler : IRequestHandler<CreatePublicationCommand, CreatePublicationResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public CreatePublicationHandler(
            IStudentRepository studentRepository,
            IPublicationRepository publicationRepository,
            IEctsTrackingRepository ectsRepository,
            IApplicationRepository applicationRepository,
            EctsProgressService ectsProgressService,
            ICurrentUserService currentUserService,
            IAuthService authService)
        {
            _studentRepository = studentRepository;
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }
        public async Task<CreatePublicationResponse> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != request.StudentId)
            {
                throw new DoctoralManagementException("You can only add publication for your own account.", HttpStatusCode.Forbidden);
            }

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);

            if (!hasAccepted)
            {
                throw new DoctoralManagementException("Student is not accepted to a doctoral program", HttpStatusCode.BadRequest);
            }

            var dateUtc = DateTime.SpecifyKind(request.PublishedOn, DateTimeKind.Utc);

            var publication = new Publication
            {
                StudentId = request.StudentId,
                Title = request.Title,
                Journal = request.Journal,
                PublishedOn = dateUtc,
                Doi = request.Doi,
                IsIndexedInScopus = request.IsIndexedInScopus,
                IsIndexedInThomsonReuters = request.IsIndexedInThomsonReuters,
            };

            var created = await _publicationRepository.AddAsync(publication);

            //var ectsTracking = await _ectsRepository.GetByStudentIdAsync(request.StudentId);
            //if (ectsTracking != null)
            //{
            //    ectsTracking.Publications += ectsPoints;
            //    if (ectsTracking.Publications > 27)
            //    {
            //        ectsTracking.Publications = 27;
            //    }
            //    await _ectsRepository.UpdateAsync(ectsTracking);
            //    await _ectsProgressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
            //}

            return new CreatePublicationResponse
            {
                Id = created.Id,
                StudentId = created.StudentId,
                Title = created.Title,
            };
        }
    }
}
