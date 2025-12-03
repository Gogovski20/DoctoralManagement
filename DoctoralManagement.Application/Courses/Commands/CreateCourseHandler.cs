using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;


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
                throw new DoctoralManagementException("Semester must be between 1 and 6", HttpStatusCode.BadRequest);
            }

            if (request.EctsCredits < 1 || request.EctsCredits > 6)
            {
                throw new DoctoralManagementException("ECTS credits must be between 1 and 6", HttpStatusCode.BadRequest);
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
