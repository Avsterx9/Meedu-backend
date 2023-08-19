using MediatR;
using Meedu.Models;

namespace Meedu.Commands.AddSubject;

public record AddSubjectCommand(
    string name) 
    : IRequest<SubjectDto>;
