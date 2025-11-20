using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, CreateCourseResponse>
    {
        private readonly ICourseRepository _courseRepository;

        public CreateCourseHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CreateCourseResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            if (request.Semester < 1 || request.Semester > 6)
            {
                throw new Exception("Semester must be between 1 and 6");
            }

            if (request.EctsCredits < 1 || request.EctsCredits > 6)
            {
                throw new Exception("ECTS credits must be between 1 and 6");
            }

            var course = new Course 
            {
                Code = request.Code,
                Title = request.Title,
                EctsCredits = request.EctsCredits,
                InstructorName = request.InstructorName,
                Semester = request.Semester,
                Description = request.Description
            };

            var created = await _courseRepository.AddAsync(course);

            return new CreateCourseResponse 
            {
                Id = created.Id,
                Code = created.Code,
                Title = created.Title
            };
        }
    }
}
