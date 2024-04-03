using AfiaNotebook.Entities.DbSet;
using AfiaNotebook.Entities.Dtos.Incoming;
using AfiaNotebook.Entities.Dtos.Outgoing;
using AutoMapper;

namespace AfiaNotebook.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDto, User>()
            .ForMember(
                dest => dest.DateOfBirth, 
                from => from.MapFrom(x => Convert.ToDateTime(x.DateOfBirth))
             );

        CreateMap<User, ProfileDto>()
            .ForMember(
                dest => dest.DateOfBirth,
                from => from.MapFrom(x => x.DateOfBirth.ToShortDateString())
            );
    }
}
