using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthBusinessRules
{
    public class AuthBusinessRules
    {
        private readonly IUserRepository _userRepo;
        private readonly EmailHelper _emailHelper;

        public AuthBusinessRules(IUserRepository userRepo, EmailHelper emailHelper)
        {
            _userRepo = userRepo;
            _emailHelper = emailHelper;
        }

        

        
    }
}

