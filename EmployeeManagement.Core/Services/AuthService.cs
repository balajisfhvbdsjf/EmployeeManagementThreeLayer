using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace EmployeeManagement.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly string _secretKey;
        private readonly Dictionary<string, (string otp, DateTime expiration)> _otpStore;

        public AuthService(IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _secretKey = configuration["Jwt:Key"];
            _otpStore = new Dictionary<string, (string otp, DateTime expiration)>();
        }

        public async Task<User> RegisterAsync(User user, string otp)
        {
            if (_otpStore.TryGetValue(user.Username, out var otpInfo) &&
                otpInfo.otp == otp && otpInfo.expiration > DateTime.UtcNow)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _otpStore.Remove(user.Username);
                return await _userRepository.AddUserAsync(user);
            }
            throw new InvalidOperationException("Invalid or expired OTP.");
        }

        public async Task<string> SendOtpAsync(string email)
        {
            var otp = GenerateOtp();
            await _emailService.SendOtpAsync(email, otp);
            // Save OTP with an expiration time ( 10 minutes)
            _otpStore[email] = (otp, DateTime.UtcNow.AddMinutes(10));
            return otp;
        }

        public async Task<string> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginRequest.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
