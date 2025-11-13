using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.ResendVerification.BusinessRules;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Auth.ResendVerification.Services
{
    public class ResendVerificationService : IResendVerificationService
    {
        private readonly UserManager<User> _userManager;
        private ResendVerificationBusinessRules _rules;


        public ResendVerificationService(UserManager<User> userManager, ResendVerificationBusinessRules rules)
        {
            _userManager = userManager;
            _rules = rules;
        }
    
        public async Task<AppError?> ResendVerificationEmailAsync(string email, string baseUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return AppError.NotFound(ErrorMessages.Get("User:UserNotFound", email));

            if (!user.RequiresEmailVerification)
                return AppError.Validation(ErrorMessages.Get("Validation:InvalidEmailVerificationRequest"));

            if (user.IsEmailVerified)
                return AppError.Conflict(ErrorMessages.Get("User:EmailAlreadyExists", user.Email!));

            await _rules.GenerateEmailVerificationAsync(user, baseUrl);
            return null;
        }
    }
}