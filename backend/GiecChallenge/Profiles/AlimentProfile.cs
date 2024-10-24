using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class AlimentProfile : Profile
    {
        public AlimentProfile() {
            CreateMap<Aliment, AlimentDto>().ForMember(dest => dest.ciqual_code, opt => { opt.MapFrom((s, d) => s.ciqual);})
                                            .ForMember(dest => dest.nom_francais, opt => { opt.MapFrom((s, d) => s.names.FirstOrDefault(n => n.language.ISOCode == "FR")?.name);})
                                            .ForMember(dest => dest.groupe, opt => { opt.MapFrom((s, d, destMember, context) => s.subgroup.names.FirstOrDefault(i => i.language == context.Items["language"])?.name);})
                                            .ForPath(dest => dest.impact_environnemental.changement_climatique.synthese, opt => { opt.MapFrom(s => s.CO2);})
                                            .ForPath(dest => dest.impact_environnemental.changement_climatique.unite, opt => { opt.MapFrom(s => s.CO2Unit);})
                                            .ForPath(dest => dest.impact_environnemental.epuisement_eau.synthese, opt => { opt.MapFrom(s => s.water);})
                                            .ForPath(dest => dest.impact_environnemental.epuisement_eau.unite, opt => { opt.MapFrom(s => s.waterUnit);});

            CreateMap<AlimentDto, Aliment>().ForMember(dest => dest.ciqual, opt => { opt.MapFrom((s, d) => s.ciqual_code);})
                                            .ForMember(dest => dest.CO2, opt => { opt.MapFrom((s, d) => s.impact_environnemental.changement_climatique.synthese);})
                                            .ForMember(dest => dest.CO2Unit, opt => { opt.MapFrom((s, d) => s.impact_environnemental.changement_climatique.unite);})
                                            .ForMember(dest => dest.water, opt => { opt.MapFrom((s, d) => s.impact_environnemental.epuisement_eau.synthese);})
                                            .ForMember(dest => dest.waterUnit, opt => { opt.MapFrom((s, d) => s.impact_environnemental.epuisement_eau.unite);})
                                            .ForMember(dest => dest.names, opt => { opt.MapFrom((s, d, destMember, context) => new List<ProductLanguage>() { new ProductLanguage() { name = s.nom_francais, language = (Language)context.Items["language"] }});});
        }
    }
}