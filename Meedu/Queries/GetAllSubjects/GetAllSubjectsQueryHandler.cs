using MediatR;
using Meedu.Models;
using Meedu.Services;

namespace Meedu.Queries.GetAllSubjects;

public sealed class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, IReadOnlyList<SubjectDto>>
{
    private readonly ISubjectService _subjectService;

    public GetAllSubjectsQueryHandler(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public async Task<IReadOnlyList<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        return await _subjectService.GetAllAsync();
    }
}
