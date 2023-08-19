using Meedu.Commands.AddSubject;
using Meedu.Models;

namespace Meedu.Services;

public interface ISubjectService
{
    Task<IReadOnlyList<SubjectDto>> GetAllAsync();
    Task<SubjectDto> AddSubjectAsync(AddSubjectCommand command);
}
