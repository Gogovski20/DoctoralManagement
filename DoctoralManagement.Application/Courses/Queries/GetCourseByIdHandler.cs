using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResponse>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseByIdHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<GetCourseByIdResponse> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException($"Course with id {request.Id} not found", HttpStatusCode.NotFound);

            return new GetCourseByIdResponse
            {
                Id = course.Id,
                Code = course.Code,
                Title = course.Title,
                EctsCredits = course.EctsCredits,
                InstructorName = course.InstructorName,
                Semester = course.Semester,
                Description = course.Description
            };
        }
    }
}
