using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, GetMyProfileResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyProfileHandler(IStudentRepository studentRepository, IMentorRepository mentorRepository, ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _mentorRepository = mentorRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetMyProfileResponse> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var email = _currentUserService.Email ?? "";
            var role = _currentUserService.Role ?? "Unknown";

            var response = new GetMyProfileResponse
            {
                UserId = userId.ToString(),
                Email = email,
                Role = role
            };

            if (role == "Student")
            {
                var student = await _studentRepository.GetByUserIdAsync(userId);

                if (student != null)
                {
                    response.StudentId = student.Id;
                    response.ExtraInfo = new
                    {
                        student.FullName,
                        student.CurrentSemester,
                        student.DoctoralProgramId,
                        student.Status
                    };
                }
            }

            if (role == "Mentor")
            {
                var mentor = await _mentorRepository.GetByUserIdAsync(userId);

                if (mentor != null)
                {
                    response.MentorId = mentor.Id;
                    response.ExtraInfo = new
                    {
                        mentor.FullName,
                        mentor.Department,
                        mentor.Title
                    };
                }
            }

            return response;
        }
    }
}
