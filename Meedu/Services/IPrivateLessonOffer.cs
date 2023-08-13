using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;

namespace Meedu.Services;

public interface IPrivateLessonService
{
    Task<PrivateLessonOfferDto> AddPrivateLessonAsync(CreateLessonOfferCommand command);
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetAllLessonOffersAsync();
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetLessonOffersByUserAsync();
    Task<DeleteLessonOfferResponse> DeleteLessonOfferAsync(DeleteLessonOfferCommand command);
    Task<PrivateLessonOfferDto> GetByIdAsync(string id);
    Task UpdateLessonOfferAsync(PrivateLessonOfferDto dto);
    Task<List<PrivateLessonOfferDto>> SimpleSearchByNameAsync(string searchValue);
    Task<List<PrivateLessonOfferDto>> AdvancedSearchAsync(LessonOfferAdvancedSearchDto dto);
}
