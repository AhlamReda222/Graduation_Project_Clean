namespace Graduation_Project.BLL.DTOs.Auth
{
    public class AuthResponseDto
    {


        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public string? Email { get; set; }

        public string? UserType { get; set; }
    }
}

