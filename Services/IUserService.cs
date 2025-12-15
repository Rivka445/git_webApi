using Entities;
using DTOs;
namespace Services
{
    public interface IUserService
    {
        Task<UserDTO> AddUser(User user);
        Task<UserDTO> GetUserById(int id);
        Task<List<UserDTO>> GetUsers();
        Task<UserLoginDTO> LogIn(User user);
        Task UpdateUser(int id, User updateUser);
    }
}