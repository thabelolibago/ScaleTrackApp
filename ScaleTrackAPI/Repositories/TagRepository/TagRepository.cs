using ScaleTrackAPI.Models;
using ScaleTrackAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace ScaleTrackAPI.Repositories
{
    public class TagRepository(AppDbContext context) : ITagRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Tag>> GetAll()
        {
            return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Tag?> GetById(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag> Add(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task Delete(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.Tags.AnyAsync(t => t.Name.ToLower() == name.ToLower());
        }
    }
}
