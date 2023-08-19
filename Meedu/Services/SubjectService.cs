using AutoMapper;
using Meedu.Entities;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAll();
        Task AddSubject(String name);
    }

    public class SubjectService : ISubjectService
    {
        private readonly MeeduDbContext _dbContext;
        private readonly IMapper _mapper;
        public SubjectService(MeeduDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<SubjectDto>> GetAll()
        {
            return await _dbContext.Subjects
                .Select(x => _mapper.Map<SubjectDto>(x))
                .ToListAsync();
        }

        public async Task AddSubject(String name)
        {
            var exists = await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == name || s.Name == name.ToLower());  

            if(exists != null)
                throw new BadHttpRequestException("SubjectAlreadyExists");
            
            await _dbContext.Subjects.AddAsync(exists);
        }
    }
}
