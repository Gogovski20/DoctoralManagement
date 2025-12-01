using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DeleteActivityDocumentHandler : IRequestHandler<DeleteActivityDocumentCommand, bool>
    {
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IFileService _fileService;

        public DeleteActivityDocumentHandler(IActivityDocumentRepository activityDocumentRepository, IFileService fileService)
        {
            _activityDocumentRepository = activityDocumentRepository;
            _fileService = fileService;
        }

        public async Task<bool> Handle(DeleteActivityDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _activityDocumentRepository.GetByIdAsync(request.ActivityDocumentId)
            ?? throw new Exception("Document not found.");

            if (document.Status == DocumentStatus.Approved)
                throw new Exception("Approved documents cannot be deleted.");

            // Make sure this document belongs to the correct activity
            if (!IsValidActivityDocument(request, document))
                throw new Exception("Document does not belong to the specified activity.");

            // Optional authorization/ownership logic here...

            // Delete file and data
            _fileService.DeleteFile(document.FilePath);
            await _activityDocumentRepository.DeleteAsync(document);

            return true;
        }

        private bool IsValidActivityDocument(DeleteActivityDocumentCommand request, ActivityDocument document)
        {
            return request.ActivityType switch
            {
                ActivityType.Publication => document.PublicationId == request.ActivityId,
                ActivityType.Mobility => document.MobilityId == request.ActivityId,
                ActivityType.Conference => document.ConferenceId == request.ActivityId,
                ActivityType.DoctoralProject => document.DoctoralProjectId == request.ActivityId,
                _ => false
            };
        }
    }
}
