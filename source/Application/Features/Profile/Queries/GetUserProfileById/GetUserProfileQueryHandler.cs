using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfileById;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, GetUserProfileByIdQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<UserFollow> _userFollowRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;

    public GetUserProfileByIdQueryHandler(IMediator mediator, IRepositoryBase<User> userRepository, CultureLocalizer localizer, IUser user, IRepositoryBase<UserProfile> userProfileRepository, IRepositoryBase<UserFollow> userFollowRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _localizer = localizer;
        _user = user;
        _userProfileRepository = userProfileRepository;
        _userFollowRepository = userFollowRepository;
    }

    public async Task<GetUserProfileByIdQueryResponse?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var user = _userRepository.Get(x => x.Id == request.Id);

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("GetUserProfileById", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var profile = _userProfileRepository.Get(x => x.UserId == user.Id);
        if (profile == null)
        {
            await _mediator.Publish(new DomainNotification("GetUserProfileById", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var followerCount = _userFollowRepository.GetRanged(x => x.FollowedUserId == user.Id).Count();
        var followingCount = _userFollowRepository.GetRanged(x => x.FollowerUserId == user.Id).Count();

        var responseViewModel = new UserProfileViewModel
        {
            Id = user.Id,
            Username = user.Username ?? string.Empty,
            DisplayName = profile.DisplayName ?? string.Empty,
            Bio = profile.Bio ?? string.Empty,
            Location = profile.Location ?? string.Empty,
            ProfilePictureUrl = profile.ProfilePictureUrl ?? string.Empty,
            Volume = profile.Volume,
            UpdatedAt = profile.UpdatedAt ?? profile.CreatedAt,
            FollowerCount = followerCount,
            FollowingCount = followingCount,
        }; 

        await _mediator.Publish(new DomainSuccessNotification("GetUserProfileById", _localizer.Text("Success")), cancellationToken);

        return new GetUserProfileByIdQueryResponse
        {
            UserProfile = responseViewModel
        };
    }
}