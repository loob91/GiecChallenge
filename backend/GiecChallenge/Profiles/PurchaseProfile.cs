using AutoMapper;
using GiecChallenge.Models;

namespace GiecChallenge.Profiles {
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile() {
            CreateMap<Purchase, PurchaseDto>().ForMember(dest => dest.products, opt => { opt.Ignore();});

            CreateMap<PurchaseDto, Purchase>().ForMember(dest => dest.user, opt => { opt.Ignore();})
                                              .ForMember(dest => dest.products, opt => opt.Ignore());

            CreateMap<ProductPurchase, ProductPurchaseDto>().ForMember(dest => dest.currencyIsoCode, opt => { opt.MapFrom((s, d) => s.currency.ISOCode);} )
                                                            .ForMember(dest => dest.product, opt => { opt.MapFrom((s, d, destMember, context) => s.product != null ? s.product.names.Any(n => n.language == (Language)context.Items["language"]) ? s.product.names.FirstOrDefault(n => n.language == (Language)context.Items["language"])?.name : s.product.names.FirstOrDefault(n => n.language.ISOCode == "FR")?.name : "");} )
                                                            .ForMember(dest => dest.productId, opt => { opt.MapFrom((s, d) => s.product?.id.ToString());} )
                                                            .ForMember(dest => dest.CO2Cost, opt => { opt.MapFrom((s, d) => s.product != null ? s.product.CO2 : 0);} )
                                                            .ForMember(dest => dest.WaterCost, opt => { opt.MapFrom((s, d) => s.product != null ? s.product.water : 0);} );
                                                         
            CreateMap<ProductPurchaseDto, ProductPurchase>().ForMember(dest => dest.id, opt => { opt.MapFrom((s, d) => !string.IsNullOrEmpty(s.id) ? Guid.Parse(s.id) : Guid.Empty );} )
                                                            .ForMember(dest => dest.product, opt => opt.Ignore() )
                                                            .ForMember(dest => dest.currency, opt => opt.Ignore() );
        }
    }
}