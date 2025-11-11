using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User?> GetByEmail(string email);
        Task<User> Add(User user);
        Task<User> Update(User user);
        Task Delete(User user);
        Task<User?> GetByVerificationToken(string token);
    }
}
