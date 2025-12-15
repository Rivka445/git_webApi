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
            CreateMap<User,UserLoginDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<Order, OrderDTO>();
            CreateMap<Product, ProductDTO>();
                                             
        }
    }
}
