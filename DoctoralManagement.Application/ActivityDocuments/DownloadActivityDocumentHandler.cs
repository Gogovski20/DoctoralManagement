//using DoctoralManagement.Domain.Entities;
//using DoctoralManagement.Domain.Interfaces;
//using MediatR;

//namespace DoctoralManagement.Application.ActivityDocuments
//{
//    public class DownloadActivityDocumentHandler : IRequestHandler<DownloadActivityDocumentQuery, DownloadDocumentResponse>
//    {
//        private readonly IActivityDocumentRepository _activityDocumentRepository;
//        private readonly IFileService _fileService;

//        public DownloadActivityDocumentHandler(IActivityDocumentRepository activityDocumentRepository, IFileService fileService)
//        {
//            _activityDocumentRepository = activityDocumentRepository;
//            _fileService = fileService;
//        }

//        public async Task<DownloadDocumentResponse> Handle(DownloadActivityDocumentQuery request, CancellationToken cancellationToken)
//        {
//            var document = await _activityDocumentRepository.GetByIdAsync(request.DocumentId)
//                ?? throw new Exception("Document not found.");

//            if (!IsValidActivityDocument(request, document))
//                throw new Exception("Document does not belong to the specified activity.");

//            var fileBytes = _fileService.ReadFile(document.FilePath);

//            return new DownloadDocumentResponse
//            {
//                FileBytes = fileBytes,
//                FileName = document.FileName,
//                ContentType = document.ContentType
//            };
//        }

//        private bool IsValidActivityDocument(DownloadActivityDocumentQuery request, ActivityDocument document)
//        {
//            return request.ActivityType switch
//            {
//                ActivityType.Publication => document.PublicationId == request.ActivityId,
//                ActivityType.Mobility => document.MobilityId == request.ActivityId,
//                ActivityType.Conference => document.ConferenceId == request.ActivityId,
//                ActivityType.DoctoralProject => document.DoctoralProjectId == request.ActivityId,
//                _ => false
//            };
//        }
//    }
//}
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DownloadActivityDocumentHandler : IRequestHandler<DownloadActivityDocumentQuery, DownloadDocumentResponse>
    {
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IFileService _fileService;

        public DownloadActivityDocumentHandler(IActivityDocumentRepository activityDocumentRepository, IFileService fileService)
        {
            _activityDocumentRepository = activityDocumentRepository;
            _fileService = fileService;
        }

        public async Task<DownloadDocumentResponse> Handle(DownloadActivityDocumentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Get document
                var document = await _activityDocumentRepository.GetByIdAsync(request.DocumentId);
                if (document == null)
                {
                    return new DownloadDocumentResponse
                    {
                        Success = false,
                        Message = "Document not found."
                    };
                }

                // 2. Validate document belongs to the specified activity
                if (!IsValidActivityDocument(request, document))
                {
                    return new DownloadDocumentResponse
                    {
                        Success = false,
                        Message = "Document does not belong to the specified activity."
                    };
                }

                // 3. Check if file exists
                if (!_fileService.FileExists(document.FilePath))
                {
                    return new DownloadDocumentResponse
                    {
                        Success = false,
                        Message = "File not found on server."
                    };
                }

                // 4. Read file
                var fileBytes = _fileService.ReadFile(document.FilePath);

                return new DownloadDocumentResponse
                {
                    Success = true,
                    FileBytes = fileBytes,
                    FileName = document.FileName,
                    ContentType = document.ContentType
                };
            }
            catch (Exception ex)
            {
                return new DownloadDocumentResponse
                {
                    Success = false,
                    Message = $"Error downloading document: {ex.Message}"
                };
            }
        }

        private bool IsValidActivityDocument(DownloadActivityDocumentQuery request, ActivityDocument document)
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