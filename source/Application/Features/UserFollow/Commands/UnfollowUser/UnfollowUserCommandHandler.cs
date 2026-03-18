using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UnfollowUser;

public class UnfollowUserCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IRepositoryBase<UserFollow> userFollowRepository,
    IRepositoryBase<User> userRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IUser user,
    CultureLocalizer localizer) : IRequestHandler<UnfollowUserCommand, UnfollowUserCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository = userFollowRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<UnfollowUserCommandResponse?> Handle(UnfollowUserCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("UnfollowUser", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        // Resolve target: accepts User.Id or UserProfile.Id
        var targetProfile = _userProfileRepository.Get(x => x.UserId == command.Request.FollowedUserId || x.Id == command.Request.FollowedUserId);
        if (targetProfile == null)
        {
            await _mediator.Publish(new DomainNotification("UnfollowUser", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var existingFollow = _userFollowRepository.Get(x => x.FollowerUserId == currentUser.Id && x.FollowedUserId == targetProfile.UserId);
        if (existingFollow == null)
        {
            await _mediator.Publish(new DomainNotification("UnfollowUser", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        _userFollowRepository.Delete(existingFollow);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("UnfollowUser", _localizer.Text("Success")), cancellationToken);
        return new UnfollowUserCommandResponse { IsFollowing = false };
    }
}
