using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;
using Meedu.Queries.GetLessonOfferById;

namespace Meedu.Services;

public interface IPrivateLessonService
{
    Task<PrivateLessonOfferDto> AddPrivateLessonAsync(CreateLessonOfferCommand command);
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetAllLessonOffersAsync();
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetLessonOffersByUserAsync();
    Task<DeleteLessonOfferResponse> DeleteLessonOfferAsync(DeleteLessonOfferCommand command);
    Task<PrivateLessonOfferDto> GetByIdAsync(GetLessonOfferByIdQuery query);
    Task UpdateLessonOfferAsync(PrivateLessonOfferDto dto);
    Task<List<PrivateLessonOfferDto>> SimpleSearchByNameAsync(string searchValue);
    Task<List<PrivateLessonOfferDto>> AdvancedSearchAsync(LessonOfferAdvancedSearchDto dto);
}
