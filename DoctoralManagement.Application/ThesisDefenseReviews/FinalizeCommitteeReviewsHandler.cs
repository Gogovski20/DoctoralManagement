using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

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

        public FinalizeCommitteeReviewsHandler(
            IThesisDefenseRepository defenseRepo,
            ICommitteeReviewRepository reviewRepo,
            IDoctoralProjectRepository projectRepo,
            IStudentRepository studentRepo,
            IEctsTrackingRepository ectsTrackingRepo)
        {
            _defenseRepo = defenseRepo;
            _reviewRepo = reviewRepo;
            _projectRepo = projectRepo;
            _studentRepo = studentRepo;
            _ectsTrackingRepo = ectsTrackingRepo;
        }

        public async Task<FinalizeCommitteeReviewsResponse> Handle(
            FinalizeCommitteeReviewsCommand request,
            CancellationToken cancellationToken)
        {
            var defense = await _defenseRepo.GetByIdAsync(request.DefenseId)
                ?? throw new Exception("Defense not found");

            if (defense.Status != DefenseStatus.Completed)
                throw new Exception("Defense must be completed BEFORE finalizing committee reviews.");

            var reviews = await _reviewRepo.GetByDefenseIdAsync(defense.Id);

            if (!reviews.Any())
                throw new Exception("Cannot finalize — no committee reviews submitted.");

            if (reviews.Count() < defense.CommitteeMemberIds.Count())
            {
                throw new Exception($"Not all committee members have submitted reviews. Expected: {defense.CommitteeMemberIds.Count()}, Received: {reviews.Count()}");
            }

            CommitteeApprovalStatus finalDecision;

            if (reviews.Any(r => r.ApprovalStatus == CommitteeApprovalStatus.Rejected))
                finalDecision = CommitteeApprovalStatus.Rejected;

            else if (reviews.Any(r => r.ApprovalStatus == CommitteeApprovalStatus.ChangesRequired))
                finalDecision = CommitteeApprovalStatus.ChangesRequired;

            else
                finalDecision = CommitteeApprovalStatus.Approved;

            var project = defense.DoctoralProject;

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

                    var student = defense.DoctoralProject.Student;
                    if (student != null)
                    {
                        var ectsTracking = await _ectsTrackingRepo.GetByStudentIdAsync(student.Id);
                        if (ectsTracking != null)
                        {
                            ectsTracking.ThesisDefence = 46;
                            await _ectsTrackingRepo.UpdateAsync(ectsTracking);

                            if (ectsTracking.TotalECTS < 180)
                            {
                                throw new Exception($"Student ECTS total is {ectsTracking.TotalECTS}. Must be 180 to graduate.");
                            }
                        }

                        student.Status = StudentStatus.Graduated;
                        await _studentRepo.UpdateAsync(student);
                    }

                    break;
            }

            await _projectRepo.UpdateAsync(project);
            await _defenseRepo.UpdateAsync(defense);

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
