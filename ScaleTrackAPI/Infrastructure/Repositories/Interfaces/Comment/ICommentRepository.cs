using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ICommentRepository
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAll(int issueId);
        Task<Comment?> GetById(int issueId, int id);
        Task<Comment> Add(Comment comment);
        Task Delete(Comment comment);
    }
}
