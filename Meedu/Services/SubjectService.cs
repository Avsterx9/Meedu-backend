using Meedu.Entities;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAll();
    }

    public class SubjectService : ISubjectService
    {
        private readonly MeeduDbContext _dbContext;
        public SubjectService(MeeduDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public async Task<List<SubjectDto>> GetAll()
        {
            var subjects = await _dbContext.Subjects.ToListAsync();
            return subjects.Select(s => new SubjectDto() { Id = s.Id.ToString(), Name = s.Name }).ToList();
        }
    }
}
