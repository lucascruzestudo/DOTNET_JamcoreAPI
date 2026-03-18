using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.IsFollowing;

public class IsFollowingQueryHandler(
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IUser user,
    CultureLocalizer localizer) : IRequestHandler<IsFollowingQuery, IsFollowingQueryResponse?>
{
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<IsFollowingQueryResponse?> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("IsFollowing", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        // Accepts User.Id or UserProfile.Id
        var targetProfile = _userProfileRepository.Get(x => x.UserId == request.FollowedUserId || x.Id == request.FollowedUserId);
        if (targetProfile == null)
        {
            await _mediator.Publish(new DomainNotification("IsFollowing", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var isFollowing = _userFollowRepository.Get(x => x.FollowerUserId == currentUser.Id && x.FollowedUserId == targetProfile.UserId) != null;

        await _mediator.Publish(new DomainSuccessNotification("IsFollowing", _localizer.Text("Success")), cancellationToken);
        return new IsFollowingQueryResponse { IsFollowing = isFollowing };
    }
}
