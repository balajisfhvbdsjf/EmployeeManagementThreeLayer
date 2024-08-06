using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace EmployeeManagement.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Your App Name", smtpSettings["Username"]));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Your OTP Code";

                message.Body = new TextPart("plain")
                {
                    Text = $"Your OTP code is: {otp}"
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpSettings["Host"], int.Parse(smtpSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log exception or handle it appropriately
                throw new Exception("Error sending OTP email", ex);
            }
        }

    }
}
