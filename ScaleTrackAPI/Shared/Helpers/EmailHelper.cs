using System.Net;
using System.Net.Mail;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Shared.Helpers
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
    <!DOCTYPE html>
    <html lang='en'>
    <head>
      <meta charset='UTF-8'>
      <meta name='viewport' content='width=device-width, initial-scale=1.0'>
      <title>Password Reset</title>
    </head>
    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin:0; padding:0;'>
      <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
          <td align='center'>
            <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
              
              <!-- Header -->
              <tr>
                <td style='background-color: #0078D4; padding: 20px; text-align: center; color: #ffffff; font-size: 24px; font-weight: bold;'>
                  Reset Your Password
                </td>
              </tr>
              
              <!-- Body -->
              <tr>
                <td style='padding: 30px; color: #333333; font-size: 16px; line-height: 1.5;'>
                  <p>Hi {user.FirstName} {user.LastName},</p>
                  <p>We received a request to reset your password. Click the button below to reset it:</p>
                  <p style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' style='background-color: #0078D4; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Reset Password</a>
                  </p>
                  <p>If you did not request a password reset, please ignore this email.</p>
                  <p>This link will expire in <strong>1 hour</strong>.</p>
                </td>
              </tr>

              <!-- Footer -->
              <tr>
                <td style='background-color: #f4f4f4; padding: 20px; text-align: center; color: #888888; font-size: 12px;'>
                  &copy; {DateTime.UtcNow.Year} ScaleTrack Technologies (Pty) Ltd. All rights reserved.<br>
                  7 Mellis Rd, Rivonia, Sandton, South Africa
                </td>
              </tr>

            </table>
          </td>
        </tr>
      </table>
    </body>
    </html>
    ";
    }

    public string BuildEmailVerificationEmail(User user, string verifyLink)
    {
      return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Verify Your Email</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f7;
                margin: 0;
                padding: 0;
            }}
            .container {{
                max-width: 600px;
                margin: 40px auto;
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            }}
            h2 {{
                color: #333333;
            }}
            p {{
                color: #555555;
                line-height: 1.6;
            }}
            a.button {{
                display: inline-block;
                padding: 12px 24px;
                margin-top: 20px;
                color: #ffffff;
                background-color: #007bff;
                border-radius: 6px;
                text-decoration: none;
                font-weight: bold;
            }}
            a.button:hover {{
                background-color: #0056b3;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 12px;
                color: #999999;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Hello {user.FirstName}{user.LastName},</h2>
            <p>Thank you for registering an account with us! To complete your registration, please verify your email by clicking the button below:</p>
            <a class='button' href='{verifyLink}'>Verify Email</a>
            <p>If you did not create this account, you can safely ignore this email.</p>
            <div class='footer'>
                &copy; {DateTime.UtcNow.Year} ScaleTrack Technologies (Pty) Ltd. All rights reserved.<br>
                  7 Mellis Rd, Rivonia, Sandton, South Africa
            </div>
        </div>
    </body>
    </html>";
    }
  }
}
