namespace MagicVilla_VillaAPI.Dto.ResponseDTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}