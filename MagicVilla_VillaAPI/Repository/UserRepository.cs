using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db,
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper) 
        {
            _roleManager = roleManager;
            _mapper =mapper;
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager=userManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(q => q.UserName == username);
            return user == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
        {
            var user = _db.ApplicationUsers
                .FirstOrDefault(q => q.UserName.ToLower() == requestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user,requestDTO.Password);
            if (user == null|| isValid==false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            //gen JWT token
            var roles= await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token=tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO requestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = requestDTO.UserName,
                Email = requestDTO.UserName,
                NormalizedEmail = requestDTO.UserName,
                Name = requestDTO.Name,
            };
            try
            {
                var result = await _userManager.CreateAsync(user, requestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));

                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn=_db.ApplicationUsers
                        .FirstOrDefault(u=>u.UserName==requestDTO.UserName);
                  
                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }catch(Exception e)
            {

            }
            return new UserDTO();
        }
    }
}
