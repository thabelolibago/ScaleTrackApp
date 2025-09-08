using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
{
    public interface IIssueTagRepository
    {
        Task<List<IssueTag>> GetAll(int issueId);
        Task<IssueTag?> Get(int issueId, int tagId);
        Task<IssueTag> Add(IssueTag issueTag);
        Task Delete(IssueTag issueTag);
    }
}
