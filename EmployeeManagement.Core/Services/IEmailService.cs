using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Mail;

namespace EmployeeManagement.Core.Services
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otp);
    }
}
