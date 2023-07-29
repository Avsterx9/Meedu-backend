using AutoMapper;
using Meedu.Entities;
using Meedu.Models.Auth;

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
	}
}
