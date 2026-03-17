using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfileComments;

public class GetUserProfileCommentsQueryHandler(
    IMediator mediator,
    IRepositoryBase<UserProfileComment> userProfileCommentRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    CultureLocalizer localizer) : IRequestHandler<GetUserProfileCommentsQuery, GetUserProfileCommentsQueryResponse?>
{
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserProfileComment> _userProfileCommentRepository = userProfileCommentRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<GetUserProfileCommentsQueryResponse?> Handle(GetUserProfileCommentsQuery request, CancellationToken cancellationToken)
    {
        var targetProfile = _userProfileRepository.Get(x => x.Id == request.UserProfileId || x.UserId == request.UserProfileId);
        if (targetProfile == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetUserProfileComments", _localizer.Text("NotFound")),
                cancellationToken
            );
            return default;
        }

        var commentsQuery = _userProfileCommentRepository
            .GetRanged(x => x.UserProfileId == targetProfile.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var commentData = (from comment in commentsQuery
                           join user in _userRepository.GetAll()
                               on comment.UserId equals user.Id
                           join commenterProfile in _userProfileRepository.GetAll()
                               on comment.UserId equals commenterProfile.UserId into commenterProfiles
                           from commenterProfile in commenterProfiles.DefaultIfEmpty()
                           select new UserProfileCommentViewModel
                           {
                               Id = comment.Id,
                               Text = comment.Comment,
                               UserProfileId = comment.UserProfileId,
                               UserId = comment.UserId,
                               Username = user.Username,
                               DisplayName = commenterProfile != null ? commenterProfile.DisplayName ?? string.Empty : string.Empty,
                               UserProfilePictureUrl = commenterProfile != null ? commenterProfile.ProfilePictureUrl : null,
                               UserProfileUpdatedAt = commenterProfile != null ? commenterProfile.UpdatedAt : null,
                               CreatedAt = comment.CreatedAt,
                           }).ToList();

        var totalCount = _userProfileCommentRepository
            .GetRanged(x => x.UserProfileId == targetProfile.Id)
            .Count();

        var paginatedComments = new PaginatedList<UserProfileCommentViewModel>(
            commentData,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetUserProfileComments", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetUserProfileCommentsQueryResponse
        {
            Comments = paginatedComments,
        };
    }
}
