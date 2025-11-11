using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueTagRepository
{
    public interface IIssueTagRepository
    {
        Task<List<IssueTag>> GetAll(int issueId);
        Task<IssueTag?> Get(int issueId, int tagId);
        Task<IssueTag> Add(IssueTag issueTag);
        Task Delete(IssueTag issueTag);
    }
}
