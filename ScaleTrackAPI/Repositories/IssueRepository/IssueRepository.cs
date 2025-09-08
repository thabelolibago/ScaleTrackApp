using ScaleTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Repositories
{
    public class IssueRepository(AppDbContext context) : IIssueRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Issue> AddIssue(Issue issue)
        {
            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        public async Task<Issue?> GetById(int id)
        {
            return await _context.Issues.FindAsync(id);
        }

        public async Task<List<Issue>> GetAll()
        {
            return await _context.Issues.ToListAsync();
        }

        public async Task<Issue?> UpdateIssue(Issue issue)
        {
            var existing = await _context.Issues.FindAsync(issue.Id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(issue);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteIssue(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
                await _context.SaveChangesAsync();
            }
        }
    }
}

