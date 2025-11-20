using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetCoursesBySemesterHandler : IRequestHandler<GetCoursesBySemesterQuery, IEnumerable<GetCourseBySemesterResponse>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCoursesBySemesterHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<GetCourseBySemesterResponse>> Handle(GetCoursesBySemesterQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetBySemesterAsync(request.Semester);

            return courses.Select(course => new GetCourseBySemesterResponse
            {
                Id = course.Id,
                Code = course.Code,
                Title = course.Title,
                EctsCredits = course.EctsCredits,
                InstructorName = course.InstructorName,
                Semester = course.Semester,
                Description = course.Description
            }).ToList();
        }
    }
}
