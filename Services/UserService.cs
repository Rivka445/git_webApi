using Entities;
using Repositories;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRipository _userRepository;

        public UserService(IUserRipository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }
        public User GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }
        public User AddUser(User user)
        {
            return _userRepository.AddUser(user);
        }
        public User LogIn(User user)
        {
            return _userRepository.LogIn(user);
        }
        public void UpdateUser(int id, User updateUser)
        {
            _userRepository.UpdateUser(id, updateUser);
        }
    }
}
