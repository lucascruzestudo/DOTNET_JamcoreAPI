using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.FollowUser;

public class FollowUserCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IUser user,
    CultureLocalizer localizer,
    IRedisService redis) : IRequestHandler<FollowUserCommand, FollowUserCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;
    private readonly IRedisService _redis = redis;

    public async Task<FollowUserCommandResponse?> Handle(FollowUserCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("FollowUser", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        // Resolve target: accepts User.Id or UserProfile.Id
        var targetProfile = _userProfileRepository.Get(x => x.UserId == command.Request.FollowedUserId || x.Id == command.Request.FollowedUserId);
        if (targetProfile == null)
        {
            await _mediator.Publish(new DomainNotification("FollowUser", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        if (targetProfile.UserId == currentUser.Id)
        {
            await _mediator.Publish(new DomainNotification("FollowUser", _localizer.Text("InvalidOperation")), cancellationToken);
            return default;
        }

        var existingFollow = _userFollowRepository.Get(x => x.FollowerUserId == currentUser.Id && x.FollowedUserId == targetProfile.UserId);
        if (existingFollow != null)
        {
            await _mediator.Publish(new DomainNotification("FollowUser", _localizer.Text("AlreadyExists")), cancellationToken);
            return default;
        }

        var userFollow = new UserFollow
        {
            FollowerUserId = currentUser.Id,
            FollowedUserId = targetProfile.UserId,
            CreatedAt = DateTime.UtcNow,
        };

        _userFollowRepository.Add(userFollow);
        _unitOfWork.Commit();

        await Task.WhenAll(
            _redis.DeleteByPrefixAsync($"follow:followers:{targetProfile.UserId}:"),
            _redis.DeleteByPrefixAsync($"follow:following:{currentUser.Id}:"),
            _redis.DeleteAsync($"follow:is:{currentUser.Id}:{targetProfile.UserId}")
        );

        await _mediator.Publish(new DomainSuccessNotification("FollowUser", _localizer.Text("Success")), cancellationToken);
        return new FollowUserCommandResponse { IsFollowing = true };
    }
}
