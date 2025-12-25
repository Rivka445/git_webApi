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
            CreateMap<User,UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<UserLoginDTO,User>();
            CreateMap<UserRegisterDTO, User>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<OrderItemDTO, OrderItem>();
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderPostDTO, Order>();
            CreateMap<Product, ProductDTO>();
        }
    }
}
