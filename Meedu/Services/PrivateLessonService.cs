﻿using AutoMapper;
using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Commands.UpdateLessonOffer;
using Meedu.Entities;
using Meedu.Entities.Enums;
using Meedu.Exceptions;
using Meedu.Helpers;
using Meedu.Models;
using Meedu.Models.Auth;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;
using Meedu.Queries.GetLessonOfferById;
using Meedu.Queries.LessonOffersSimpleSearch;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services;

public class PrivateLessonService : IPrivateLessonService
{
    private readonly MeeduDbContext _context;
    private readonly ILogger<PrivateLessonService> _logger;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public PrivateLessonService(
        MeeduDbContext dbContext,
        ILogger<PrivateLessonService> logger,
        IUserContextService userContextService,
        IMapper mapper)
    {
        _context = dbContext;
        _logger = logger;
        _userContextService = userContextService;
        _mapper = mapper;
    }

    public async Task<PrivateLessonOfferDto> AddPrivateLessonAsync(CreateLessonOfferCommand command)
    {
        var userId = _userContextService.GetUserIdFromToken();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name.Contains(command.Subject.Name));

        if (subject == null)
            subject = new Subject() { Name = command.Subject.Name };

        var newLesson = _mapper.Map<PrivateLessonOffer>(command);

        newLesson.CreatedById = userId;
        newLesson.Subject = subject;

        await _context.PrivateLessonOffers.AddAsync(newLesson);
        await _context.SaveChangesAsync();

        return _mapper.Map<PrivateLessonOfferDto>(newLesson);
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> GetAllLessonOffersAsync()
    {
        return await _context.PrivateLessonOffers
            .Include(s => s.Subject)
            .Include(s => s.CreatedBy)
            .ThenInclude(x => x.Image)
            .Select(x => _mapper.Map<PrivateLessonOfferDto>(x))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> GetLessonOffersByUserAsync()
    {
        var userId = _userContextService.GetUserIdFromToken();

         return await _context.PrivateLessonOffers
            .Include(s => s.Subject)
            .Include(s => s.CreatedBy)
            .ThenInclude(x => x.Image)
            .Where(x => x.CreatedById == userId)
            .Select(x => _mapper.Map<PrivateLessonOfferDto>(x))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DeleteLessonOfferResponse> DeleteLessonOfferAsync(DeleteLessonOfferCommand command)
    {
        var userId = _userContextService.GetUserIdFromToken();

        var lessonOffer = await _context.PrivateLessonOffers
            .FirstOrDefaultAsync(o => o.Id == command.LessonId)
            ?? throw new NotFoundException(ExceptionMessages.LessonOfferNotFound);

        var reservations = await _context.LessonReservations
            .Where(x => x.PrivateLessonOfferId == lessonOffer.Id)
            .AsNoTracking()
            .ToListAsync();

        _context.RemoveRange(reservations);
        _context.PrivateLessonOffers.Remove(lessonOffer);
        await _context.SaveChangesAsync();

        return new DeleteLessonOfferResponse(true, "Lesson removed successfully");
    }

    public async Task<PrivateLessonOfferDto> GetByIdAsync(GetLessonOfferByIdQuery query)
    {
        var lesson = await _context.PrivateLessonOffers
            .Include(x => x.CreatedBy)
            .ThenInclude(x => x.Image)
            .Include(x => x.Subject)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == query.LessonId)
            ?? throw new NotFoundException(ExceptionMessages.LessonOfferNotFound);

        return _mapper.Map<PrivateLessonOfferDto>(lesson);
    }

    public async Task<PrivateLessonOfferDto> UpdateLessonOfferAsync(UpdateLessonOfferCommand command)
    {
        var offerToEdit = await _context.PrivateLessonOffers
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == command.Id)
            ?? throw new NotFoundException(ExceptionMessages.LessonOfferNotFound);

        if(command.Subject.Id != offerToEdit.SubjectId)
        {
            var selectedSubject = await _context.Subjects
                .FirstOrDefaultAsync(x => x.Id == command.Subject.Id);

            if (selectedSubject == null)
                selectedSubject = new Subject { Name = command.Subject.Name };

            offerToEdit.SubjectId = selectedSubject.Id;
        }

        EntityHelper.UpdateEntity(command, offerToEdit);

        await _context.SaveChangesAsync();
        return _mapper.Map<PrivateLessonOfferDto>(offerToEdit);
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> SimpleSearchByNameAsync(SearchLessonOffersQuery query)
    {
        return await _context.PrivateLessonOffers
            .Include(x => x.CreatedBy)
            .ThenInclude(x => x.Image)
            .Include(x => x.Subject)
            .Where(x => x.LessonTitle.ToLower().Contains(query.Value.ToLower()))
            .Select(x => _mapper.Map<PrivateLessonOfferDto>(x))
            .ToListAsync();
    }

    public async Task<List<PrivateLessonOfferDto>> AdvancedSearchAsync(LessonOfferAdvancedSearchDto dto)
    {
        var lessons = await _context.PrivateLessonOffers
            .Include(o => o.CreatedBy)
            .ThenInclude(x => x.Image)
            .Include(o => o.Subject)
            .ToListAsync();

        if (!string.IsNullOrEmpty(dto.Subject))
            lessons = lessons.Where(o => o.Subject.Name.Equals(dto.Subject)).ToList();

        if (!string.IsNullOrEmpty(dto.LastName))
            lessons = lessons.Where(o => o.CreatedBy.LastName.Equals(dto.LastName)).ToList();

        if (!string.IsNullOrEmpty(dto.FirstName))
            lessons = lessons.Where(o => o.CreatedBy.FirstName.Equals(dto.FirstName)).ToList();

        if (!string.IsNullOrEmpty(dto.City))
            lessons = lessons.Where(o => o.City.Equals(dto.City)).ToList();

        if (dto.TeachingRange != null)
            lessons = lessons.Where(o => o.TeachingRange == dto.TeachingRange).ToList();

        if (dto.IsOnline != null)
            lessons = lessons.Where(o => o.IsRemote == dto.IsOnline).ToList();

        if (dto.PriceFrom != null)
            lessons = lessons.Where(o => o.Price > dto.PriceFrom).ToList();

        if (dto.PriceTo != null)
            lessons = lessons.Where(o => o.Price < dto.PriceTo).ToList();

        return lessons.Select(o => CreateLessonOfferDto(o)).ToList();
    }

    private PrivateLessonOfferDto CreateLessonOfferDto(PrivateLessonOffer offer)
    {
        return new PrivateLessonOfferDto()
        {
            Id = offer.Id.ToString(),
            City = offer.City,
            Description = offer.Description != null ? offer.Description : "",
            isOnline = offer.IsRemote,
            LessonTitle = offer.LessonTitle,
            Place = offer.Place,
            Price = offer.Price,
            Subject = new DtoNameId()
            {
                Id = offer.Subject.Id.ToString(),
                Name = offer.Subject.Name
            },
            TeachingRange = offer.TeachingRange != null ? offer.TeachingRange : TeachingRange.Other,
            User = new DtoNameLastnameId()
            {
                Id = offer.CreatedBy.Id.ToString(),
                FirstName = offer.CreatedBy.FirstName,
                LastName = offer.CreatedBy.LastName,
                PhoneNumber = offer.CreatedBy.PhoneNumber,
                ImageDto = new ImageDto()
                {
                    ContentType = offer.CreatedBy.Image == null ? "" : offer.CreatedBy.Image.ContentType,
                    Data = offer.CreatedBy.Image == null ? "" : Convert.ToBase64String(offer.CreatedBy.Image.Data)
                }
            }
        };
    }
}
