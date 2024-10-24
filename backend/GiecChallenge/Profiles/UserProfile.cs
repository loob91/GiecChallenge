using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<User, UserDto>().ForMember(dest => dest.token, opt => { opt.Ignore();})
                                      .ForMember(dest => dest.password, opt => { opt.Ignore();});

            CreateMap<UserDto, User>().ForMember(dest => dest.creationDate, opt => { opt.MapFrom((s, d) => DateTime.Now);})
                                      .ForMember(dest => dest.id, opt => opt.Ignore());

            CreateMap<UserGroup, UserGroupDto>().ForMember(dest => dest.id, opt => opt.Ignore() );
                                                         
            CreateMap<UserGroupDto, UserGroup>();
        }
    }
}