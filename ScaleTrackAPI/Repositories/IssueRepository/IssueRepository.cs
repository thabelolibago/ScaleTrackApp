using ScaleTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly AppDbContext _context;

        public IssueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Issue> AddIssue(Issue issue)
        {
            await _context.Issues.AddAsync(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        public async Task<Issue?> GetById(int id)
        {
            // Include CreatedBy to avoid null references in the mapper
            return await _context.Issues
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Issue>> GetAll()
        {
            // Include CreatedBy for consistent response mapping
            return await _context.Issues
                .Include(i => i.CreatedBy)
                .ToListAsync();
        }

        public async Task<Issue?> UpdateIssue(Issue issue)
        {
            var existing = await _context.Issues.FindAsync(issue.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(issue);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteIssue(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
                return;

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();
        }
    }
}

