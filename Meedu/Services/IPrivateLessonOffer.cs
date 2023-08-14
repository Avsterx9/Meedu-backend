using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Commands.UpdateLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;
using Meedu.Queries.GetLessonOfferById;
using Meedu.Queries.LessonOffersSimpleSearch;

namespace Meedu.Services;

public interface IPrivateLessonService
{
    Task<PrivateLessonOfferDto> AddPrivateLessonAsync(CreateLessonOfferCommand command);
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetAllLessonOffersAsync();
    Task<IReadOnlyList<PrivateLessonOfferDto>> GetLessonOffersByUserAsync();
    Task<DeleteLessonOfferResponse> DeleteLessonOfferAsync(DeleteLessonOfferCommand command);
    Task<PrivateLessonOfferDto> GetByIdAsync(GetLessonOfferByIdQuery query);
    Task<PrivateLessonOfferDto> UpdateLessonOfferAsync(UpdateLessonOfferCommand command);
    Task<IReadOnlyList<PrivateLessonOfferDto>> SimpleSearchByNameAsync(SearchLessonOffersQuery query);
    Task<List<PrivateLessonOfferDto>> AdvancedSearchAsync(LessonOfferAdvancedSearchDto dto);
}
