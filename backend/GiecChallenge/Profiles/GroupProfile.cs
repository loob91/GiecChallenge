using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class GroupProfile : Profile
    {
        public GroupProfile() {
            CreateMap<ProductGroup, GroupDto>().ForMember(dest => dest.names, opt => { opt.MapFrom((s, d) => s.names);})
                                               .ForMember(dest => dest.language, opt => { opt.Ignore();});

            CreateMap<GroupDto, ProductGroup>().ForMember(dest => dest.names, opt => opt.Ignore())
                                               .ForMember(dest => dest.id, opt => opt.Ignore());

            CreateMap<ProductGroupLanguage, GroupNamesDto>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                            .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d) => s.language.ISOCode);});
                                                         
            CreateMap<GroupNamesDto, ProductGroupLanguage>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                         .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d, destMember, context) => (Language)context.Items["language"]);});
        }
    }
}