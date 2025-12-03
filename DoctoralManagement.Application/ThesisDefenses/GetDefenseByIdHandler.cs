using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetDefenseByIdHandler : IRequestHandler<GetDefenseByIdQuery, GetDefenseByIdResponse>
    {
        private readonly IThesisDefenseRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetDefenseByIdHandler> _logger;

        public GetDefenseByIdHandler(IThesisDefenseRepository repository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetDefenseByIdHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetDefenseByIdResponse> Handle(GetDefenseByIdQuery request, CancellationToken cancellationToken)
        {
            var defense = await _repository.GetByIdAsync(request.DefenseId)
                ?? throw new DoctoralManagementException("Defense not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == defense.DoctoralProject?.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId == defense.DoctoralProject?.MentorId;
            bool isCommittee = currentUserRole == "Committee" && defense.CommitteeMemberIds.Contains(currentUserId);
            bool isSecretary = currentUserRole == "Secretary";

            if (!isOwner && !isAdmin && !isMentor && !isCommittee && !isSecretary)
            {
                throw new DoctoralManagementException(
                    "You don't have permission to view this defense.",
                    HttpStatusCode.Forbidden);
            }

            _logger.LogInformation(
                "{Role} {UserId} viewed thesis defense {DefenseId}",
                currentUserRole, currentUserId, request.DefenseId);

            return new GetDefenseByIdResponse
            {
                Id = defense.Id,
                ProjectId = defense.DoctoralProjectId,
                StudentName = defense.DoctoralProject?.Student?.FullName,
                ScheduledAt = defense.ScheduledAt,
                Room = defense.Room,
                CommitteeMemberCount = defense.CommitteeMemberIds?.Count() ?? 0,
                Status = defense.Status.ToString(),
            };
        }
    }
}
