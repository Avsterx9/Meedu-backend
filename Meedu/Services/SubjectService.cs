using AutoMapper;
using Meedu.Commands.AddSubject;
using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Helpers;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services;

public class SubjectService : ISubjectService
{
    private readonly MeeduDbContext _context;
    private readonly IMapper _mapper;

    public SubjectService(MeeduDbContext dbContext, IMapper mapper)
    {
        _context = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SubjectDto>> GetAllAsync()
    {
        return await _context.Subjects
            .Select(x => _mapper.Map<SubjectDto>(x))
            .ToListAsync();
    }

    public async Task<SubjectDto> AddSubjectAsync(AddSubjectCommand command)
    {
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name == command.name || s.Name == command.name.ToLower());  

        if(existingSubject != null)
            throw new BadRequestException(ExceptionMessages.SubjectAlreadyExists);

        var newSubject = new Subject
        {
            Name = command.name,
        };

        await _context.Subjects.AddAsync(newSubject);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<SubjectDto>(newSubject);
    }
}
