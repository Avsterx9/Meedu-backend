using MediatR;
using Meedu.Models;

namespace Meedu.Queries.GetUserStudents;

public record GetUserStudentsQuery(int Amount) 
    : IRequest<IReadOnlyList<DtoNameLastnameId>>;
