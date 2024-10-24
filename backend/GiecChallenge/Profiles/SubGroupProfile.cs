using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class SubGroupProfile : Profile
    {
        public SubGroupProfile() {
            CreateMap<ProductSubGroup, SubGroupDto>().ForMember(dest => dest.names, opt => { opt.MapFrom((s, d) => s.names);})
                                                     .ForMember(dest => dest.language, opt => { opt.Ignore();})
                                                     .ForMember(dest => dest.group, opt => { opt.MapFrom((s, d) => s.Groupe.names.FirstOrDefault(n => n.language.ISOCode == d.language)?.name); });

            CreateMap<SubGroupDto, ProductSubGroup>().ForMember(dest => dest.names, opt => opt.Ignore())
                                                     .ForMember(dest => dest.id, opt => opt.Ignore())
                                                     .ForMember(dest => dest.Groupe, opt => opt.Ignore());

            CreateMap<ProductSubGroupLanguage, SubGroupNamesDto>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                                  .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d) => s.language.ISOCode);});
                                                         
            CreateMap<SubGroupNamesDto, ProductSubGroupLanguage>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                                  .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d, destMember, context) => (Language)context.Items["language"]);});
        }
    }
}