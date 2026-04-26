using AutoMapper;
using CameraShop.API.DTOs.Brand;
using CameraShop.API.DTOs.Camera;
using CameraShop.API.DTOs.Cart;
using CameraShop.API.DTOs.Category;
using CameraShop.API.DTOs.Order;
using CameraShop.API.Models;

namespace CameraShop.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Brand, BrandDto>();
        CreateMap<BrandUpsertDto, Brand>();
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryUpsertDto, Category>();
        CreateMap<Camera, CameraDto>()
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.Categories,
                o => o.MapFrom(s => s.CameraCategories.Select(cc => cc.Category.Name).ToList()));

        CreateMap<CameraUpsertDto, Camera>()
            .ForMember(d => d.CameraCategories, o => o.Ignore());
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.CameraModel, o => o.MapFrom(s => s.Camera.Model));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.Username))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.CameraModel, o => o.MapFrom(s => s.Camera.Model))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Camera.ImageUrl))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.Camera.Price));

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
    }
}
