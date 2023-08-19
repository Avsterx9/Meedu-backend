using MediatR;
using Meedu.Models;

namespace Meedu.Queries.GetAllSubjects;

public record GetAllSubjectsQuery() : IRequest<IReadOnlyList<SubjectDto>>;
