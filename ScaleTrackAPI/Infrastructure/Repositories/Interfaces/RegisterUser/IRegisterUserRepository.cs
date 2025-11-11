using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.RegisterUser
{
    public interface IRegisterUserRepository
    {
        Task<User> RegisterUserAsync(User user);
    }
}