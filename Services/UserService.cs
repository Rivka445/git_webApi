using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Logging;
using Repositories;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<bool> IsExistsUserById(int id)
        {
            return await _userRepository.IsExistsUserById(id);
        }
        public bool CheckUser(int id)
        {
            return true;
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            List<User> users = await _userRepository.GetUsers();
            List<UserDTO> usersDTO = _mapper.Map<List<User>, List<UserDTO>>(users);
            return usersDTO;
        }
        public async Task<UserDTO> GetUserById(int id)
        {
            User? user= await _userRepository.GetUserById(id);
            if (user == null)
                return null;
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
        public async Task<UserDTO> AddUser(UserRegisterDTO newUser)
        {
            User userRegister = _mapper.Map<UserRegisterDTO, User>(newUser);
            User user = await _userRepository.AddUser(userRegister);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            if (userDTO != null)
                _logger.LogInformation("user id: {id} register succecfully", userDTO.Id);
            return userDTO;
        }
        public async Task<UserDTO> LogIn(UserLoginDTO existUser)
        {
            User loginUser = _mapper.Map<UserLoginDTO,User>(existUser);
            User? user = await _userRepository.LogIn(loginUser);
            if (user == null)
                return null;
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }

        public async Task UpdateUser(int id, UserRegisterDTO updateUser)
        {
            User user = _mapper.Map<UserRegisterDTO, User>(updateUser);
            user.Id = id;
            await _userRepository.UpdateUser(user);
        }
    }
}
