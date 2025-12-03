using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class FinalizeCommitteeReviewsHandler :
        IRequestHandler<FinalizeCommitteeReviewsCommand, FinalizeCommitteeReviewsResponse>
    {
        private readonly IThesisDefenseRepository _defenseRepo;
        private readonly ICommitteeReviewRepository _reviewRepo;
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IEctsTrackingRepository _ectsTrackingRepo;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<FinalizeCommitteeReviewsHandler> _logger;

        public FinalizeCommitteeReviewsHandler(
            IThesisDefenseRepository defenseRepo,
            ICommitteeReviewRepository reviewRepo,
            IDoctoralProjectRepository projectRepo,
            IStudentRepository studentRepo,
            IEctsTrackingRepository ectsTrackingRepo,
            EctsProgressService ectsProgressService,
            ICurrentUserService currentUserService,
            ILogger<FinalizeCommitteeReviewsHandler> logger)
        {
            _defenseRepo = defenseRepo;
            _reviewRepo = reviewRepo;
            _projectRepo = projectRepo;
            _studentRepo = studentRepo;
            _ectsTrackingRepo = ectsTrackingRepo;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<FinalizeCommitteeReviewsResponse> Handle(
            FinalizeCommitteeReviewsCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserRole = _currentUserService.Role;
            if (currentUserRole != "Admin")
            {
                throw new DoctoralManagementException(
                    "Only admins can finalize committee reviews.",
                    HttpStatusCode.Forbidden);
            }

            var defense = await _defenseRepo.GetByIdAsync(request.DefenseId)
                ?? throw new Exception("Defense not found");

            if (defense.Status != DefenseStatus.Completed)
                throw new DoctoralManagementException("Defense must be completed BEFORE finalizing committee reviews.", HttpStatusCode.NotFound);

            var reviews = await _reviewRepo.GetByDefenseIdAsync(defense.Id);

            if (!reviews.Any())
                throw new DoctoralManagementException("Cannot finalize — no committee reviews submitted.", HttpStatusCode.BadRequest);

            if (reviews.Count() < defense.CommitteeMemberIds.Count())
            {
                throw new DoctoralManagementException($"Not all committee members have submitted reviews. Expected: {defense.CommitteeMemberIds.Count()}, Received: {reviews.Count()}", HttpStatusCode.BadRequest);
            }

            CommitteeApprovalStatus finalDecision;

            if (reviews.Any(r => r.ApprovalStatus == CommitteeApprovalStatus.Rejected))
                finalDecision = CommitteeApprovalStatus.Rejected;

            else if (reviews.Any(r => r.ApprovalStatus == CommitteeApprovalStatus.ChangesRequired))
                finalDecision = CommitteeApprovalStatus.ChangesRequired;

            else
                finalDecision = CommitteeApprovalStatus.Approved;

            var project = defense.DoctoralProject ?? throw new DoctoralManagementException(
                    "Doctoral project not found for this defense.",
                    HttpStatusCode.NotFound);

            switch (finalDecision)
            {
                case CommitteeApprovalStatus.Rejected:
                    project.Status = ProjectStatus.DefenseRejected;
                    defense.Status = DefenseStatus.Failed;
                    break;

                case CommitteeApprovalStatus.ChangesRequired:
                    project.Status = ProjectStatus.DefenseChangesRequired;
                    defense.Status = DefenseStatus.Completed;
                    break;

                case CommitteeApprovalStatus.Approved:
                    project.Status = ProjectStatus.DefenseApproved;
                    defense.Status = DefenseStatus.Passed;
                    defense.ResultNotes = "Thesis defense successful";

                    var student = defense.DoctoralProject.Student ?? throw new DoctoralManagementException(
                            "Student not found for this project.",
                            HttpStatusCode.NotFound);
                    if (student != null)
                    {
                        var ectsTracking = await _ectsTrackingRepo.GetByStudentIdAsync(student.Id) ?? throw new DoctoralManagementException(
                            "ECTS tracking record not found for student.",
                            HttpStatusCode.NotFound);
                        if (ectsTracking != null)
                        {
                            ectsTracking.ThesisDefence += 26;
                            if (ectsTracking.ThesisDefence > 46)
                            {
                                ectsTracking.ThesisDefence = 46;
                            }
                            await _ectsTrackingRepo.UpdateAsync(ectsTracking);

                            if (ectsTracking.TotalECTS < 180)
                            {
                                throw new Exception($"Student ECTS total is {ectsTracking.TotalECTS}. Must be 180 to graduate.");
                            }
                            await _ectsProgressService.UpdateStudentSemesterAsync(student.Id, ectsTracking.TotalECTS);
                        }

                        student.Status = StudentStatus.Graduated;
                        await _studentRepo.UpdateAsync(student);

                        _logger.LogInformation("Student {StudentId} graduated successfully", student.Id);
                    }

                    break;
            }

            await _projectRepo.UpdateAsync(project);
            await _defenseRepo.UpdateAsync(defense);

            _logger.LogInformation(
                "Committee reviews finalized for defense {DefenseId}. Final decision: {Decision}",
                defense.Id, finalDecision);

            return new FinalizeCommitteeReviewsResponse
            {
                DefenseId = defense.Id,
                FinalDecision = finalDecision.ToString(),
                Reviews = reviews.Select(r => new CommitteeReviewResponse
                {
                    Id = r.Id,
                    ThesisDefenseId = r.ThesisDefenseId,
                    ReviewerId = r.ReviewerId,
                    Comments = r.Comments,
                    ApprovalStatus = r.ApprovalStatus.ToString(),
                    ReviewedAt = r.ReviewedAt
                }).ToList()
            };
        }
    }
}
