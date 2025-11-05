using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Helpers
{
    public class PasswordHelper
    {
        private readonly string _pepper;
        private readonly PasswordHasher<User> _hasher;

        public PasswordHelper(IConfiguration config)
        {
            _pepper = config["Security:PasswordPepper"] ?? string.Empty;
            _hasher = new PasswordHasher<User>();
        }

        // Append pepper before storing
        public string WithPepper(string password) => password + _pepper;

        // Verify a plain password against the hashed password in database
        public bool VerifyWithPepper(string plainPassword, string hashedPassword, User? user = null)
        {
            var passwordWithPepper = plainPassword + _pepper;
            var result = _hasher.VerifyHashedPassword(user ?? new User(), hashedPassword, passwordWithPepper);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
