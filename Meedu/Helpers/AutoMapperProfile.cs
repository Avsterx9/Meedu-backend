using AutoMapper;
using Meedu.Commands.CreateLessonOffer;
using Meedu.Entities;
using Meedu.Models;
using Meedu.Models.Auth;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;

namespace Meedu.Helpers;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
		CreateMap<User, UserInfoDto>();

		CreateMap<User, UserInfoDto>()
			.ForMember(x => x.ImageDto, o => o.MapFrom(src => new ImageDto
			{
				ContentType = src.Image == null ? string.Empty : src.Image.ContentType,
				Data = src.Image == null ? string.Empty : Convert.ToBase64String(src.Image.Data)
            }));

		CreateMap<CreateLessonOfferCommand, PrivateLessonOffer>()
			.ForMember(x => x.IsRemote,
				o => o.MapFrom(src => src.isOnline))
			.ForMember(x => x.Subject,
				o => o.MapFrom(src => new Subject()));

        CreateMap<PrivateLessonOffer, PrivateLessonOfferDto>()
			.ForMember(x => x.isOnline,
				o => o.MapFrom(src => src.IsRemote))
			.ForMember(x => x.Subject,
				o => o.MapFrom(src => new DtoNameId
				{
					Id = src.SubjectId,
					Name = src.Subject.Name
				}))
			.ForMember(x => x.User,
				o => o.MapFrom(src => src.CreatedBy == null ? null : new DtoNameLastnameId
				{
					Id = src.CreatedBy.Id,
					FirstName = src.CreatedBy.FirstName,
					LastName = src.CreatedBy.LastName,
					PhoneNumber = src.CreatedBy.PhoneNumber,
					ImageDto = new ImageDto
					{
						ContentType = src.CreatedBy.Image == null ? string.Empty : src.CreatedBy.Image.ContentType,
						Data = src.CreatedBy.Image == null ? string.Empty : Convert.ToBase64String(src.CreatedBy.Image.Data)
					}
				}));


        CreateMap<LessonReservation, UserReservationDataDto>()
			.ForMember(x => x.AvailableFrom, o => o.MapFrom(src => src.ScheduleTimespan.AvailableFrom))
			.ForMember(x => x.AvailableTo, o => o.MapFrom(src => src.ScheduleTimespan.AvailableTo))
			.ForMember(x => x.ReservationId, o => o.MapFrom(src => src.Id.ToString()))
			.ForMember(x => x.ScheduleId, o => o.MapFrom(src => src.ScheduleTimespan.DaySchedule.Id.ToString()))
			.ForMember(x => x.TimespanId, o => o.MapFrom(src => src.ScheduleTimespan.Id.ToString()));

		CreateMap<Subject, SubjectDto>();

		CreateMap<DaySchedule, ScheduleDto>()
			.ForMember(x => x.ScheduleTimestamps, o => o.MapFrom(src => src.ScheduleTimestamps));

        CreateMap<ScheduleTimestamp, ScheduleTimespanDto>();

		CreateMap<LessonReservation, LessonReservationDto>()
			.ForMember(x => x.ReservedBy,
				o => o.MapFrom(src => new DtoNameId
				{
					Id = src.ReservedById,
					Name = $"{src.ReservedBy.FirstName} {src.ReservedBy.LastName}"
				}));
    }
}
