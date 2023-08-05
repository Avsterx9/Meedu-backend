using AutoMapper;
using Meedu.Entities;
using Meedu.Models;
using Meedu.Models.Auth;
using Meedu.Models.Reservations.UserReservations;

namespace Meedu.Helpers;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
		CreateMap<RegisterUserDto, User>();

		CreateMap<User, UserInfoDto>()
			.ForMember(x => x.ImageDto, o => o.MapFrom(src => new ImageDto
			{
				ContentType = src.Image == null ? string.Empty : src.Image.ContentType,
				Data = src.Image == null ? string.Empty : Convert.ToBase64String(src.Image.Data)
            }));

		CreateMap<LessonReservation, UserReservationDataDto>()
			.ForMember(x => x.AvailableFrom, o => o.MapFrom(src => src.ScheduleTimespan.AvailableFrom.ToString("HH:mm")))
			.ForMember(x => x.AvailableTo, o => o.MapFrom(src => src.ScheduleTimespan.AvailableTo.ToString("HH:mm")))
			//.ForMember(x => x.isOnline, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.OnlineLessonsPossible))
			//.ForMember(x => x.LessonTitle, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.LessonTitle))
			//.ForMember(x => x.lessonId, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Id))
			//.ForMember(x => x.Place, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Place))
			.ForMember(x => x.ReservationId, o => o.MapFrom(src => src.Id.ToString()))
			.ForMember(x => x.ScheduleId, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.Id.ToString()))
			.ForMember(x => x.TimespanId, o => o.MapFrom(src => src.ScheduleTimespan.Id.ToString()));
			//.ForMember(x => x.User, o => o.MapFrom(src => new DtoNameLastnameId
			//{
			//	Id = src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.Id.ToString(),
			//	FirstName = src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.FirstName,
			//	LastName = src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.LastName,
			//	PhoneNumber = src.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.PhoneNumber,
			//}));
	}
}
