using Meedu.Commands.CreateLessonOffer;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Services;

public interface IPrivateLessonService
{
    Task<PrivateLessonOfferDto> AddPrivateLessonAsync(CreateLessonOfferCommand command);
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetAllLessonOffersAsync();
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetLessonOffersByUserAsync();
    Task DeleteLessonOfferAsync(string id);
    Task<PrivateLessonOfferDto> GetByIdAsync(string id);
    Task UpdateLessonOfferAsync(PrivateLessonOfferDto dto);
    Task<List<PrivateLessonOfferDto>> SimpleSearchByNameAsync(string searchValue);
    Task<List<PrivateLessonOfferDto>> AdvancedSearchAsync(LessonOfferAdvancedSearchDto dto);
}
