using AutoMapper;
using Meedu.Entities;
using Meedu.Models.Auth;

namespace Meedu.Helpers;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
		CreateMap<RegisterUserDto, User>();
	}
}
