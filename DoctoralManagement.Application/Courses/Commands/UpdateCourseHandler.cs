using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, bool>
    {
        private readonly ICourseRepository _courseRepository;

        public UpdateCourseHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException("Course not found", HttpStatusCode.NotFound);

            course.Code = request.Code;
            course.Title = request.Title;
            course.EctsCredits = request.EctsCredits;
            course.InstructorName = request.InstructorName;
            course.Semester = request.Semester;
            course.Description = request.Description;

            await _courseRepository.UpdateAsync(course);

            return true;
        }
    }
}
