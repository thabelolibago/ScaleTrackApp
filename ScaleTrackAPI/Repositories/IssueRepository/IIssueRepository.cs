using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
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

