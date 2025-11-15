using Entities;
using Repositories;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRipository _userRipository;

        public UserService(IUserRipository userRipository)
        {
            _userRipository = userRipository;
        }

        public List<User> GetUsers()
        {
            return _userRipository.GetUsers();
        }
        public User GetUserById(int id)
        {
            return _userRipository.GetUserById(id);
        }
        public User AddUser(User user)
        {
            return _userRipository.AddUser(user);
        }
        public User LogIn(User user)
        {
            return _userRipository.LogIn(user);
        }
        public void UpdateUser(int id, User updateUser)
        {
            _userRipository.UpdateUser(id, updateUser);
        }

    }
}
