using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.Tag;

namespace ScaleTrackAPI.Mappers
{
    public static class TagMapper
    {
        public static TagResponse ToResponse(Tag tag) => new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        };

        public static Tag ToModel(TagRequest request) => new Tag
        {
            Name = request.Name
        };
    }
}
