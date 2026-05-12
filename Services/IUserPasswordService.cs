namespace Services
{
    public interface IUserPasswordService
    {
        int CheckPassword(string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}