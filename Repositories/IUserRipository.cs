using Entities;

namespace Repositories
{
    public interface IUserRipository
    {
        User AddUser(User user);
        User GetUserById(int id);
        List<User> GetUsers();
        User LogIn(User user);
        void UpdateUser(int id, User updateUser);
    }
}