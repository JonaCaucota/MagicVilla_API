using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Dto;
using MagicVilla_VillaAPI.Dto.RequestDTO;
using MagicVilla_VillaAPI.Dto.ResponseDTO;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private string secretKey;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration)
    {
        _db = db;
        _userManager = userManager;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        _mapper = mapper;
    }

    public bool IsUniqueUser(string username)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
        if(user == null)
        {
            return true;
        }

        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
        if (user == null || isValid == false)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("xd");

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {

                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDto = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = _mapper.Map<UserDTO>(user),
            Role = roles.FirstOrDefault()
        };

        return loginResponseDto;

    }

    public async Task<UserDTO> Register(RegistrationRequestDTO registerationRequestDTO)
    {
        ApplicationUser user = new()
        {
            UserName = registerationRequestDTO.UserName,
            Email = registerationRequestDTO.UserName,
            Name = registerationRequestDTO.Name,
            NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
        };
        try
        {
            var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "admin");
                var userToReturn =
                    _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                return _mapper.Map<UserDTO>(userToReturn);
            }
        }
        catch (Exception e)
        {
            
        }

        return new UserDTO();
    }
}