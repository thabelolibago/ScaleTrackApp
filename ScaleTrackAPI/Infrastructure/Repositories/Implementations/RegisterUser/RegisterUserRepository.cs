using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.RegisterUser;

namespace ScaleTrackAPI.Infrastructure.Repositories.Implementations.RegisterUser
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        private readonly AppDbContext _context;

        public RegisterUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}