using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;

    public GetUserProfileQueryHandler(IMediator mediator, IRepositoryBase<User> userRepository, CultureLocalizer localizer, IUser user, IRepositoryBase<UserProfile> userProfileRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _localizer = localizer;
        _user = user;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<GetUserProfileQueryResponse?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = _userRepository.Get(x => x.Id == _user.Id);

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("GetUserProfile", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var profile = _userProfileRepository.Get(x => x.UserId == user.Id);
        if (profile == null)
        {
            await _mediator.Publish(new DomainNotification("GetUserProfile", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var responseViewModel = new UserProfileViewModel
        {
            Id = _user.Id ?? Guid.Empty,
            Username = user.Username ?? string.Empty,
            DisplayName = profile.DisplayName ?? string.Empty,
            Bio = profile.Bio ?? string.Empty,
            Location = profile.Location ?? string.Empty,
            ProfilePictureUrl = profile.ProfilePictureUrl ?? string.Empty,
            UpdatedAt = profile.UpdatedAt ?? profile.CreatedAt
        }; 

        await _mediator.Publish(new DomainSuccessNotification("GetUserProfile", _localizer.Text("Success")), cancellationToken);

        return new GetUserProfileQueryResponse
        {
            UserProfile = responseViewModel
        };
    }
}