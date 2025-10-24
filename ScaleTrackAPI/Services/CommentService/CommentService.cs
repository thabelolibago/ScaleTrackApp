using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Messages;

namespace ScaleTrackAPI.Services
{
    public class CommentService(ICommentRepository repo, IIssueRepository issueRepo, IValidator<CommentRequest> validator)
    {
        private readonly ICommentRepository _repo = repo;
        private readonly IIssueRepository _issueRepo = issueRepo;
        private readonly IValidator<CommentRequest> _validator = validator;

        public async Task<List<CommentResponse>> GetCommentsByIssueIdAsync(int issueId)
        {
            var comments = await _repo.GetAll(issueId);
            return comments.Select(CommentMapper.ToResponse).ToList();
        }

        public async Task<(CommentResponse? Response, AppError? Error)> GetById(int issueId, int id)
        {
            var comment = await _repo.GetById(issueId, id);
            if (comment == null)
                return (null, AppError.NotFound(ErrorMessages.Get("CommentNotFound", id)));

            return (CommentMapper.ToResponse(comment), null);
        }

        public async Task<(CommentResponse? Response, AppError? Error)> AddCommentAsync(int issueId, CommentRequest request)
        {
            var issue = await _issueRepo.GetById(issueId);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("IssueForCommentNotFound", issueId)));

            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var comment = CommentMapper.ToModel(issueId, request);
            var created = await _repo.Add(comment);

            if (created == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")));

            return (CommentMapper.ToResponse(created), null);
        }

        public async Task<(AppError? Error, string? Message)> DeleteCommentAsync(int issueId, int id)
        {
            var comment = await _repo.GetById(issueId, id);
            if (comment == null)
                return (AppError.NotFound(ErrorMessages.Get("CommentNotFound", id)), null);

            await _repo.Delete(comment);
            return (null, SuccessMessages.Get("CommentDeleted"));
        }
    }
}
