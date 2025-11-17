using Entities;
using System.Reflection.PortableExecutable;
using System.Text.Json;
namespace Repositories
{
    public class UserRipository : IUserRipository
    {
        string _filePath = "D:\\webAPI\\WebApiShop\\users.txt";
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    users.Add(userFromFile);
                }
            }
            return users;
        }
        public User GetUserById(int id)
        {
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (userFromFile != null && userFromFile.Id == id)
                        return userFromFile;
                }
            }
            return null;
        }
        public User AddUser(User user)
        {
            int numberOfUsers = System.IO.File.ReadLines(_filePath).Count();
            user.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(user);
            System.IO.File.AppendAllText(_filePath, userJson + Environment.NewLine);
            return user;
        }
        public User LogIn(User user)
        {
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (userFromFile != null && userFromFile.UserName == user.UserName && userFromFile.Password == user.Password)
                        return userFromFile;
                }
            }
            return null;
        }
        public void UpdateUser(int id, User updateUser)
        {
            string textToReplace = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (userFromFile != null && userFromFile.Id == id)
                        textToReplace = currentUserInFile;
                }
            }
            if (textToReplace != string.Empty)
            {
                string text = System.IO.File.ReadAllText(_filePath);
                text = text.Replace(textToReplace, JsonSerializer.Serialize(updateUser));
                System.IO.File.WriteAllText(_filePath, text);
            }
        }
    }
}
