using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using System.Text.Json;

namespace Project.Application.Features.Commands.IsFollowing;

public class IsFollowingQueryHandler(
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IUser user,
    CultureLocalizer localizer,
    IRedisService redis) : IRequestHandler<IsFollowingQuery, IsFollowingQueryResponse?>
{
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;
    private readonly IRedisService _redis = redis;

    public async Task<IsFollowingQueryResponse?> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("IsFollowing", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var cacheKey = $"follow:is:{currentUser.Id}:{request.FollowedUserId}";
        var cached = await _redis.GetAsync<IsFollowingQueryResponse>(cacheKey);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<IsFollowingQueryResponse>(cached);
            if (cachedResult != null)
            {
                await _mediator.Publish(new DomainSuccessNotification("IsFollowing", _localizer.Text("Success")), cancellationToken);
                return cachedResult;
            }
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
        var response = new IsFollowingQueryResponse { IsFollowing = isFollowing };
        await _redis.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return response;
    }
}
