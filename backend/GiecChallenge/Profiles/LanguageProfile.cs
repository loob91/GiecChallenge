using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class LanguageProfile : Profile
    {
        public LanguageProfile() {
            CreateMap<Language, LanguageDto>().ForMember(dest => dest.names, opt => { opt.MapFrom((s, d) => s.names);})
                                               .ForMember(dest => dest.language, opt => { opt.Ignore();});

            CreateMap<LanguageDto, Language>().ForMember(dest => dest.names, opt => opt.Ignore())
                                               .ForMember(dest => dest.id, opt => opt.Ignore());

            CreateMap<LanguageLanguage, LanguageNamesDto>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                            .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d) => s.language.ISOCode);});
                                                         
            CreateMap<LanguageNamesDto, LanguageLanguage>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                         .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d, destMember, context) => (Language)context.Items["language"]);});
        }
    }
}