using DoctoralManagement.Domain.Interfaces;
using MediatR;

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
                ?? throw new Exception($"Course with id {request.Id} not found");

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
