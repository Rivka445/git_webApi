using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DTOs;
using Repositories;
namespace Services
{
    public class AutoMapping : Profile
    {
        public AutoMapping() 
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<Category, NewCategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(d => d.ModelName, o => o.MapFrom(s => s.Dress.Model.Name));
            CreateMap<OrderItemDTO, OrderItem>()
                .ForMember(dest => dest.DressId, opt => opt.MapFrom(src => src.DressId));
            CreateMap<OrderItem, NewOrderItemDTO>()
                .ForMember(dest => dest.DressId, opt => opt.MapFrom(src => src.DressId));
            CreateMap<NewOrderItemDTO, OrderItem>();
            CreateMap<NewOrderDTO, Order>()
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.EventDate))
            .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.FinalPrice))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));
            CreateMap<Order, NewOrderDTO>();
            CreateMap<Order, OrderDTO>()
                 .ForMember(d => d.StatusName,o => o.MapFrom(s => s.Status.Name))
                 .ForMember(d => d.UserFirstName,o => o.MapFrom(s => s.User.FirstName))
                 .ForMember(d => d.UserLastName, o => o.MapFrom(s => s.User.LastName));
            CreateMap<OrderDTO, Order>();
            CreateMap<Dress, DressDTO>()
                .ForMember(d => d.ModelName, o => o.MapFrom(s => s.Model.Name))
                .ForMember(d => d.ModelImgUrl, o => o.MapFrom(s => s.Model.ImgUrl));
            CreateMap<DressDTO, Dress>()
                .ForPath(d => d.Model.Name, o => o.MapFrom(s => s.ModelName));
            CreateMap<Dress, NewDressDTO>().ReverseMap();
            CreateMap<NewDressDTO, Dress>()
                .ForMember(d => d.ModelId, o => o.MapFrom(s => s.ModelId))
                .ForPath(d => d.Model.Id, o => o.MapFrom(s => s.ModelId));
            CreateMap<Dress, NewDressDTO>().ReverseMap();
            CreateMap<Model, ModelDTO>().ReverseMap();
            CreateMap<Model, NewModelDTO>();
            CreateMap<NewModelDTO, Model>()
                 .ForMember(d => d.Categories, o => o.MapFrom(s => s.CategoriesId.Select(id => new Category { Id = id }).ToList()));

        }
    }
}
