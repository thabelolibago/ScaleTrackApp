using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User?> GetByEmail(string email);
        Task<User> Add(User user);
        Task Delete(User user);
    }
}
