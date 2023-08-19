using MediatR;
using Meedu.Models;
using Meedu.Services;

namespace Meedu.Commands.AddSubject;

public sealed class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, SubjectDto>
{
    private readonly ISubjectService _subjectService;

    public AddSubjectCommandHandler(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public async Task<SubjectDto> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
    {
        return await _subjectService.AddSubjectAsync(request);
    }
}
