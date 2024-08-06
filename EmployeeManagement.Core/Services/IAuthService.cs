using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Models;
using System.Threading.Tasks;

namespace EmployeeManagement.Core.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(User user, string otp);
        Task<string> SendOtpAsync(string email);
        Task<string> LoginAsync(LoginRequest loginRequest);
    }
}
