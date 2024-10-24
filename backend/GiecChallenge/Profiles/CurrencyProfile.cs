using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile() {
            CreateMap<Currency, CurrencyDto>().ForMember(dest => dest.names, opt => { opt.MapFrom((s, d) => s.names);})
                                               .ForMember(dest => dest.language, opt => { opt.Ignore();});

            CreateMap<CurrencyDto, Currency>().ForMember(dest => dest.names, opt => opt.Ignore())
                                               .ForMember(dest => dest.id, opt => opt.Ignore());

            CreateMap<CurrencyLanguage, CurrencyNamesDto>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                            .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d) => s.language.ISOCode);});
                                                         
            CreateMap<CurrencyNamesDto, CurrencyLanguage>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                         .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d, destMember, context) => (Language)context.Items["language"]);});
        }
    }
}