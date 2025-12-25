using AutoMapper;
using Entities;
using DTOs;
using Repositories;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRipository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRipository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            List<User> users = await _userRepository.GetUsers();
            List<UserDTO> usersDTO = _mapper.Map<List<User>, List<UserDTO>>(users);
            return usersDTO;
        }
        public async Task<UserDTO> GetUserById(int id)
        {
            User user= await _userRepository.GetUserById(id);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
        public async Task<UserDTO> AddUser(UserRegisterDTO newUser)
        {
            User userRegister= _mapper.Map<UserRegisterDTO, User>(newUser);
            User user = await _userRepository.AddUser(userRegister);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
        public async Task<UserDTO> LogIn(UserLoginDTO existUser)
        {
            User loginUser= _mapper.Map<UserLoginDTO,User>(existUser);
            User user = await _userRepository.LogIn(loginUser);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
        public async Task UpdateUser(int id, UserDTO updateUser)
        {
            User user = _mapper.Map<UserDTO,User>(updateUser);
            await _userRepository.UpdateUser(id, user);
        }
    }
}
