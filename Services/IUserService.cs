using Entities;
using DTOs;
namespace Services
{
    public interface IUserService
    {
        Task<bool> IsExistsUserById(int id);
        Task<UserDTO> AddUser(UserRegisterDTO user);
        Task<UserDTO> GetUserById(int id);
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> LogIn(UserLoginDTO user);
        Task UpdateUser(int id, UserDTO updateUser);
    }
}