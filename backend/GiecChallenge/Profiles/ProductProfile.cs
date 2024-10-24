using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class ProductProfile : Profile
    {
        public ProductProfile() {
            CreateMap<Product, ProductDto>().ForMember(dest => dest.id, opt => { opt.MapFrom((s, d) => s.id);})
                                            .ForMember(dest => dest.names, opt => { opt.MapFrom((s, d) => s.names);})
                                            .ForMember(dest => dest.group, opt => { opt.MapFrom((s, d, destMember, context) => s.subgroup.names.FirstOrDefault(i => i.language.ISOCode == d.language)?.name);});

            CreateMap<ProductDto, Product>().ForMember(dest => dest.names, opt => opt.Ignore())
                                            .ForMember(dest => dest.subgroup, opt => opt.Ignore());

            CreateMap<ProductLanguage, ProductNamesDto>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                         .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d) => s.language.ISOCode);});
                                                         
            CreateMap<ProductNamesDto, ProductLanguage>().ForMember(dest => dest.name, opt => { opt.MapFrom((s, d) => s.name);})
                                                         .ForMember(dest => dest.language, opt => { opt.MapFrom((s, d, destMember, context) => (Language)context.Items["language"]);});

            CreateMap<ProductUserTranslation, ProductUserTranslationDTO>().ForMember(dest => dest.product, opt => { opt.MapFrom((s, d) => s.product.id.ToString());})
                                                                          .ForMember(dest => dest.user, opt => { opt.MapFrom((s, d) => s.user.id.ToString());});
                                                         
            CreateMap<ProductUserTranslationDTO, ProductUserTranslation>().ForMember(dest => dest.id, opt => { opt.Ignore();})
                                                                          .ForMember(dest => dest.product, opt => { opt.Ignore();})
                                                                          .ForMember(dest => dest.user, opt => { opt.Ignore(); });
        }
    }
}