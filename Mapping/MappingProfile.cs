using AutoMapper;
using Daraz_CloneAgain.DTOs;
using Daraz_CloneAgain.Models;

namespace Daraz_CloneAgain.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product <-> ProductDto
            CreateMap<Product, ProductDto>().ReverseMap();

            // Child collections mappings
            CreateMap<ProductImages, ProductImageDto>().ReverseMap();
            CreateMap<ProductColors, ProductColorDto>().ReverseMap();
            CreateMap<ProductStorageOptions, ProductStorageOptionDto>().ReverseMap();
            CreateMap<ProductDelivery, ProductDeliveryDto>().ReverseMap();
            CreateMap<ProductWarranty, ProductWarrantyDto>().ReverseMap();
            CreateMap<ProductSeller, ProductSellerDto>().ReverseMap();
        }
    }
}
