using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DigiBankContext _context;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public AuthController(DigiBankContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    // POST: api/Auth/signup
    [HttpPost("signup")]
    public async Task<ActionResult> Signup(SignupRequest request)
    {
        if (await _context.Profiles.AnyAsync(p => p.Email == request.Email))
            return BadRequest("Email already in use");

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var profile = new Profile
        {
            Email = request.Email,
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(request.MotDePasse),
            ClientId = null,
            Otp = otp,
            OtpExpiry = DateTime.Now.AddMinutes(5)
        };

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpAsync(request.Email, otp);

        return Ok("OTP sent to your email");
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Email == request.Email);

        if (profile == null || !BCrypt.Net.BCrypt.Verify(request.MotDePasse, profile.MotDePasse))
            return Unauthorized("Invalid email or password");

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        profile.Otp = otp;
        profile.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpAsync(request.Email, otp);

        return Ok("OTP sent to your email");
    }

    // POST: api/Auth/verify-otp
    [HttpPost("verify-otp")]
    public async Task<ActionResult<AuthResponse>> VerifyOtp(VerifyOtpRequest request)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Email == request.Email);

        if (profile == null)
            return NotFound("Profile not found");

        if (profile.Otp != request.Otp)
            return BadRequest("Invalid OTP");

        if (profile.OtpExpiry < DateTime.UtcNow)
            return BadRequest("OTP has expired");

        // Clear OTP after successful verification
        profile.Otp = null;
        profile.OtpExpiry = null;
        await _context.SaveChangesAsync();

        var token = GenerateToken(profile);

        return Ok(new AuthResponse
        {
            Token = token,
            ProfileId = profile.Id,
            Email = profile.Email,
            ClientId = profile.ClientId
        });
    }

    private string GenerateToken(Profile profile)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString()),
            new Claim(ClaimTypes.Email, profile.Email),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:TokenValidityInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}