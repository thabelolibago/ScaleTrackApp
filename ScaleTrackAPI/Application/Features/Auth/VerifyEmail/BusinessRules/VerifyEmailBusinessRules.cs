using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.BusinessRules
{
    public class VerifyEmailBusinessRules
    {
        private readonly IUserRepository _userRepo;
        public VerifyEmailBusinessRules(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
  
        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var user = await _userRepo.GetByVerificationToken(token);
            if (user == null)
                return (false, ErrorMessages.Get("Auth:InvalidToken"));

            user.IsEmailVerified = true;
            user.RequiresEmailVerification = false;
            user.EmailVerificationToken = null;
            user.EmailVerifiedAt = DateTime.UtcNow;

            await _userRepo.Update(user);

            return (true, SuccessMessages.Get("Email:EmailVerified"));
        }
    }
}