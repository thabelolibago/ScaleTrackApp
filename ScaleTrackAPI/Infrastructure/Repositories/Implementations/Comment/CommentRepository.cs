using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ICommentRepository;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Implementations.CommentRepository
{
    public class CommentRepository(AppDbContext context) : ICommentRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Comment>> GetAll(int issueId)
        {
            return await _context.Comments
                .Where(c => c.IssueId == issueId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetById(int issueId, int id)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(c => c.IssueId == issueId && c.Id == id);
        }

        public async Task<Comment> Add(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
