using ScaleTrackAPI.Application.Features.Tags.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Tags.Mappers.TagMapper
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
