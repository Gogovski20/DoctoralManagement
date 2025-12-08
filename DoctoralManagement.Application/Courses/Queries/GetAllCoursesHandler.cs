using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, IEnumerable<GetAllCoursesResponse>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetAllCoursesHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<GetAllCoursesResponse>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync();

            return courses.Select(course => new GetAllCoursesResponse
            {
                Id = course.Id,
                Code = course.Code,
                Title = course.Title,
                EctsCredits = course.EctsCredits,
                InstructorName = course.InstructorName,
                Semester = course.Semester,
                Description = course.Description,
            });
        }
    }
}
