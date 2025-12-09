using Entities;
using Repositories;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _userRepository.GetUsers();
        }
        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }
        public async Task<User> AddUser(User user)
        {
            return await _userRepository.AddUser(user);
        }
        public async Task<User> LogIn(User user)
        {
            return await _userRepository.LogIn(user);
        }
        public async Task UpdateUser(int id, User updateUser)
        {
            await _userRepository.UpdateUser(id, updateUser);
        }
    }
}
