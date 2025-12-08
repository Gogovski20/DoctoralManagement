using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, bool>
    {
        private readonly ICourseRepository _courseRepository;

        public DeleteCourseHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException("Course not found", HttpStatusCode.NotFound);

            await _courseRepository.DeleteAsync(course.Id);

            return true;
        }
    }
}
