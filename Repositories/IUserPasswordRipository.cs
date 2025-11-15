using Entities;

namespace Repositories
{
    public interface IUserPasswordRipository
    {
        int CheckPassword(UserPassword password);
    }
}