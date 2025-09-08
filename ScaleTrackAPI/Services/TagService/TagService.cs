using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Errors;

namespace ScaleTrackAPI.Services
{
    public class TagService(ITagRepository repo, IValidator<TagRequest> validator)
    {
        private readonly ITagRepository _repo = repo;
        private readonly IValidator<TagRequest> _validator = validator;

        public async Task<List<TagResponse>> GetAllTags()
        {
            var tags = await _repo.GetAll();
            return tags.Select(TagMapper.ToResponse).ToList();
        }

        public async Task<TagResponse?> GetById(int id)
        {
            var tag = await _repo.GetById(id);
            return tag == null ? null : TagMapper.ToResponse(tag);
        }

        public async Task<(TagResponse? Response, AppError? Error)> CreateTag(TagRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            if (await _repo.ExistsByName(request.Name))
                return (null, AppError.Conflict($"Tag '{request.Name}' already exists."));

            var tag = TagMapper.ToModel(request);
            var created = await _repo.Add(tag);
            return (TagMapper.ToResponse(created), null);
        }

        public async Task<AppError?> DeleteTag(int id)
        {
            var tag = await _repo.GetById(id);
            if (tag == null)
                return AppError.NotFound($"Tag with id {id} not found.");

            await _repo.Delete(tag);
            return null;
        }
    }
}
