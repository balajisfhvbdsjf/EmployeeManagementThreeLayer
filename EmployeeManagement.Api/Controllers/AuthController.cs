using EmployeeManagement.Core.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user, [FromQuery] string otp)
        {
            var registeredUser = await _authService.RegisterAsync(user, otp);
            return Ok(registeredUser);
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string email)
        {
            var otp = await _authService.SendOtpAsync(email);
            return Ok(new { Otp = otp });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var token = await _authService.LoginAsync(loginRequest);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(new { Token = token });
        }
    }
}
