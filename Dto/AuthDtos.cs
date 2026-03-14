namespace TodoApi.DTOs;

public class SignupRequest
{
    public string Email { get; set; } = null!;
    public string MotDePasse { get; set; } = null!;
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string MotDePasse { get; set; } = null!;
}

public class VerifyOtpRequest
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public int ProfileId { get; set; }
    public string Email { get; set; } = null!;
    public int? ClientId { get; set; }
}