using ScaleTrackAPI.Models;
using ScaleTrackAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace ScaleTrackAPI.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.OrderBy(u => u.FirstName).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByVerificationToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        }
    }
}
