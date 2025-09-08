using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAll();
        Task<Tag?> GetById(int id);
        Task<Tag> Add(Tag tag);
        Task Delete(Tag tag);
        Task<bool> ExistsByName(string name);
    }
}
