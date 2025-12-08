using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
    {
        private readonly IAuthService _authService;
        private readonly IStudentRepository _studentRepository;
        private readonly IMentorRepository _mentorRepository;

        public RegisterHandler(IAuthService authService, IStudentRepository studentRepository, IMentorRepository mentorRepository)
        {
            _authService = authService;
            _studentRepository = studentRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (await _authService.UserExistsAsync(request.Email))
            {
                return new RegisterResponse { Success = false, Message = "This email is already registered." };
            }

            int? studentId = null;
            int? mentorId = null;

            if (request.Role == Domain.Entities.UserRole.Student)
            {
                var student = await _studentRepository.GetByEmailAsync(request.Email);
                if (student == null)
                {
                    return new RegisterResponse { Success = false, Message = "No student record found for this email." };
                }

                if (await _authService.IsStudentLinkedAsync(student.Id))
                {
                    return new RegisterResponse { Success = false, Message = "This student is already linked to a user account." };
                }

                studentId = student.Id;
            }
            else if (request.Role == Domain.Entities.UserRole.Mentor)
            {
                var mentor = await _mentorRepository.GetByEmailAsync(request.Email);
                if (mentor == null)
                {
                    return new RegisterResponse { Success = false, Message = "No mentor record found for this email." };
                }

                if (await _authService.IsMentorLinkedAsync(mentor.Id))
                {
                    return new RegisterResponse { Success = false, Message = "This mentor is already linked to a user account." };
                }

                mentorId = mentor.Id;
            }
            else
            {
                return new RegisterResponse { Success = false, Message = "Self-registration is not allowed for this role. Contact an administrator." };
            }

            // 1. Create Identity User
            var userId = await _authService.CreateUserAsync(
                new RegisterUserDto(request.FullName, request.Email, request.Role),
                request.Password,
                studentId,
                mentorId
            );

            // 2. Link reverse FK: Student.ApplicationUserId
            if (studentId.HasValue)
            {
                var student = await _studentRepository.GetByIdAsync(studentId.Value);
                student.ApplicationUserId = userId;
                await _studentRepository.UpdateAsync(student);
            }

            // 3. Link reverse FK: Mentor.ApplicationUserId
            if (mentorId.HasValue)
            {
                var mentor = await _mentorRepository.GetByIdAsync(mentorId.Value);
                mentor.ApplicationUserId = userId;
                await _mentorRepository.UpdateAsync(mentor);
            }

            return new RegisterResponse { Success = true, Message = "Registration successful." };
        }
    }
}
