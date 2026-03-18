using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetFollowing;

public class GetFollowingQueryHandler(
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    CultureLocalizer localizer) : IRequestHandler<GetFollowingQuery, GetFollowingQueryResponse?>
{
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<GetFollowingQueryResponse?> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        // Accepts User.Id or UserProfile.Id
        var targetProfile = _userProfileRepository.Get(x => x.UserId == request.UserId || x.Id == request.UserId);
        if (targetProfile == null)
        {
            await _mediator.Publish(new DomainNotification("GetFollowing", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var followsQuery = _userFollowRepository
            .GetRanged(x => x.FollowerUserId == targetProfile.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var data = (from follow in followsQuery
                    join followedUser in _userRepository.GetAll()
                        on follow.FollowedUserId equals followedUser.Id
                    join followedProfile in _userProfileRepository.GetAll()
                        on follow.FollowedUserId equals followedProfile.UserId into followedProfiles
                    from followedProfile in followedProfiles.DefaultIfEmpty()
                    select new UserFollowViewModel
                    {
                        UserId = followedUser.Id,
                        Username = followedUser.Username,
                        DisplayName = followedProfile != null ? followedProfile.DisplayName ?? string.Empty : string.Empty,
                        ProfilePictureUrl = followedProfile != null ? followedProfile.ProfilePictureUrl : null,
                        ProfilePictureUpdatedAt = followedProfile != null ? followedProfile.UpdatedAt : null,
                        FollowedAt = follow.CreatedAt,
                        FollowerCount = _userFollowRepository.GetRanged(x => x.FollowedUserId == followedUser.Id).Count(),
                    }).ToList();

        var totalCount = _userFollowRepository
            .GetRanged(x => x.FollowerUserId == targetProfile.UserId)
            .Count();

        var paginated = new PaginatedList<UserFollowViewModel>(data, totalCount, request.PageNumber, request.PageSize);

        await _mediator.Publish(new DomainSuccessNotification("GetFollowing", _localizer.Text("Success")), cancellationToken);
        return new GetFollowingQueryResponse { Following = paginated };
    }
}
