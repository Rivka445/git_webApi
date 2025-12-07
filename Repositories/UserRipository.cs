using Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Text.Json;
namespace Repositories
{
    public class UserRipository : IUserRipository
    {
        private readonly WebApiShopContext _webApiShopContext;
        public UserRipository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }
        public async Task<List<User>> GetUsers()
        {
            return await _webApiShopContext.Users.ToListAsync();   
        }
        public async Task<User> GetUserById(int id)
        {   
            return await _webApiShopContext.Users.FindAsync(id);
        }
        public async Task<User> AddUser(User user)
        {
            await _webApiShopContext.Users.AddAsync(user);
            await _webApiShopContext.SaveChangesAsync();
            return user;
        }
        public async Task<User> LogIn(User user)
        {
            User currentUser = await _webApiShopContext.Users.FirstOrDefaultAsync(u=> u.UserName == user.UserName && u.Password == user.Password);
            if (currentUser !=null )
                return currentUser;
             return null;
        }
        public async Task UpdateUser(int id, User updateUser)
        {
            _webApiShopContext.Users.Update(updateUser);
            await _webApiShopContext.SaveChangesAsync();
        }
    }
}
