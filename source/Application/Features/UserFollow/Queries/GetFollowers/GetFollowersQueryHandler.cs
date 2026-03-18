using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetFollowers;

public class GetFollowersQueryHandler(
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    CultureLocalizer localizer) : IRequestHandler<GetFollowersQuery, GetFollowersQueryResponse?>
{
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<GetFollowersQueryResponse?> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        // Accepts User.Id or UserProfile.Id
        var targetProfile = _userProfileRepository.Get(x => x.UserId == request.UserId || x.Id == request.UserId);
        if (targetProfile == null)
        {
            await _mediator.Publish(new DomainNotification("GetFollowers", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var followsQuery = _userFollowRepository
            .GetRanged(x => x.FollowedUserId == targetProfile.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var data = (from follow in followsQuery
                    join followerUser in _userRepository.GetAll()
                        on follow.FollowerUserId equals followerUser.Id
                    join followerProfile in _userProfileRepository.GetAll()
                        on follow.FollowerUserId equals followerProfile.UserId into followerProfiles
                    from followerProfile in followerProfiles.DefaultIfEmpty()
                    select new UserFollowViewModel
                    {
                        UserId = followerUser.Id,
                        Username = followerUser.Username,
                        DisplayName = followerProfile != null ? followerProfile.DisplayName ?? string.Empty : string.Empty,
                        ProfilePictureUrl = followerProfile != null ? followerProfile.ProfilePictureUrl : null,
                        ProfilePictureUpdatedAt = followerProfile != null ? followerProfile.UpdatedAt : null,
                        FollowedAt = follow.CreatedAt,
                        FollowerCount = _userFollowRepository.GetRanged(x => x.FollowedUserId == followerUser.Id).Count(),
                    }).ToList();

        var totalCount = _userFollowRepository
            .GetRanged(x => x.FollowedUserId == targetProfile.UserId)
            .Count();

        var paginated = new PaginatedList<UserFollowViewModel>(data, totalCount, request.PageNumber, request.PageSize);

        await _mediator.Publish(new DomainSuccessNotification("GetFollowers", _localizer.Text("Success")), cancellationToken);
        return new GetFollowersQueryResponse { Followers = paginated };
    }
}
