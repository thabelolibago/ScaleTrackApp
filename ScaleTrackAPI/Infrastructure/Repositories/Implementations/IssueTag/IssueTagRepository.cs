using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueTagRepository;

namespace ScaleTrackAPI.Infrastructure.Repositories.Implementations.IssueTagRepository
{
    public class IssueTagRepository(AppDbContext context) : IIssueTagRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<IssueTag>> GetAll(int issueId)
        {
            return await _context.IssueTags
                .Where(it => it.IssueId == issueId)
                .ToListAsync();
        }

        public async Task<IssueTag?> Get(int issueId, int tagId)
        {
            return await _context.IssueTags
                .FirstOrDefaultAsync(it => it.IssueId == issueId && it.TagId == tagId);
        }

        public async Task<IssueTag> Add(IssueTag issueTag)
        {
            _context.IssueTags.Add(issueTag);
            await _context.SaveChangesAsync();
            return issueTag;
        }

        public async Task Delete(IssueTag issueTag)
        {
            _context.IssueTags.Remove(issueTag);
            await _context.SaveChangesAsync();
        }
    }
}
