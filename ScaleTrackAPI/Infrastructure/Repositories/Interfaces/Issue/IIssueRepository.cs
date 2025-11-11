using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueRepository
{
    public interface IIssueRepository
    {
        Task<Issue> AddIssue(Issue issue);
        Task<Issue?> GetById(int id);
        Task<List<Issue>> GetAll();
        Task<Issue?> UpdateIssue(Issue issue);
        Task DeleteIssue(int id);
    }
}

