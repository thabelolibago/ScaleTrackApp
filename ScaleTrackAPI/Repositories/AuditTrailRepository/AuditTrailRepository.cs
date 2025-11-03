using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
{
    public class AuditTrailRepository(AppDbContext context) : IAuditTrailRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<AuditTrail>> GetAll()
        {
            return await _context.AuditTrails
                .Where(a => !a.IsDeleted) // filter out soft-deleted
                .Include(a => a.ChangedByUser)
                .Include(a => a.ApprovedByUser)
                .OrderByDescending(a => a.ChangedAt)
                .ToListAsync();
        }

        public async Task<AuditTrail?> GetById(int id)
        {
            return await _context.AuditTrails
                .Where(a => !a.IsDeleted) // filter out soft-deleted
                .Include(a => a.ChangedByUser)
                .Include(a => a.ApprovedByUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AuditTrail> Add(AuditTrail audit)
        {
            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
            return audit;
        }

        public async Task<AuditTrail?> Update(AuditTrail audit)
        {
            var existing = await _context.AuditTrails.FindAsync(audit.Id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(audit);
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
