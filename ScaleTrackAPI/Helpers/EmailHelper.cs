using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Helpers
{
    /// <summary>
    /// Helper class for sending emails and building email content.
    /// </summary>
    public class EmailHelper
    {
        private readonly IConfiguration _config;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Sends an email asynchronously using SMTP configuration from appsettings.json.
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body (HTML allowed)</param>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Validate parameters
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentNullException(nameof(to), "Recipient email cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body), "Email body cannot be null or empty.");

            // Read SMTP settings from configuration
            var smtpSection = _config.GetSection("SmtpSettings");
            var host = smtpSection["Host"];
            var portStr = smtpSection["Port"];
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];
            var fromEmail = smtpSection["FromEmail"];

            // Validate SMTP settings
            if (string.IsNullOrWhiteSpace(host))
                throw new Exception("SMTP Host is not configured in appsettings.json");
            if (!int.TryParse(portStr, out int port))
                throw new Exception("SMTP Port is not configured correctly in appsettings.json");
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("SMTP Username is not configured in appsettings.json");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("SMTP Password is not configured in appsettings.json");
            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new Exception("SMTP FromEmail is not configured in appsettings.json");

            // Create and configure SMTP client
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            // Create the email message
            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "ScaleTrack Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            // Send email
            await client.SendMailAsync(mail);
        }

        // -------------------------------------------------
        // Build the HTML body for a password reset email
        // -------------------------------------------------
        /// <summary>
        /// Generates the HTML content for a password reset email.
        /// </summary>
        /// <param name="user">User receiving the email</param>
        /// <param name="resetLink">Reset link URL</param>
        /// <returns>HTML string of the email body</returns>
        public string BuildPasswordResetEmail(User user, string resetLink)
        {
            return $@"
                   <h3>Password Reset Request</h3>
                   <p>Hi {user.FirstName} {user.LastName},</p>
                   <p>Click the link below to reset your password:</p>
                   <a href='{resetLink}'>Reset Password</a>
                   <p>This link expires in 1 hour.</p>";
        }
    }
}
