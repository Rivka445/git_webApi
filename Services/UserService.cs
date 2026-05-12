using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;
        private readonly IUserPasswordService _userPasswordService;


        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, IConfiguration config, IUserPasswordService userPasswordService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
            _userPasswordService = userPasswordService;
        }
        private string GenerateToken(string role, int userId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role) 
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
        public async Task<AuthResponseDto> AddUser(UserRegisterDTO newUser)
        {
            User userRegister = _mapper.Map<UserRegisterDTO, User>(newUser);
            userRegister.Password = _userPasswordService.HashPassword(newUser.Password);
            User user = await _userRepository.AddUser(userRegister);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            if (userDTO != null)
                _logger.LogInformation("user id: {id} register succecfully", userDTO.Id);
            string token = GenerateToken(newUser.Role, userDTO.Id);
            return new AuthResponseDto
            {
                User = userDTO,
                Token = token
            };
        }
        public async Task<AuthResponseDto> LogIn(UserLoginDTO existUser)
        {
            User loginUser = _mapper.Map<UserLoginDTO,User>(existUser);
            User? user = await _userRepository.LogIn(loginUser);
            if (user == null)
                return null;
            bool isValid = _userPasswordService.VerifyPassword(loginUser.Password, user.Password);

            if (!isValid)
                return null;

            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            string token = GenerateToken(existUser.Role, userDTO.Id);
            return new AuthResponseDto
            {
                User = userDTO,
                Token = token
            };
        }

        public async Task UpdateUser(int id, UserRegisterDTO updateUser)
        {
            User user = _mapper.Map<UserRegisterDTO, User>(updateUser);
            user.Id = id;
            await _userRepository.UpdateUser(user);
        }
    }
}
