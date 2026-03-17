using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateUserProfileComment;

public class CreateUserProfileCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IRepositoryBase<UserProfileComment> userProfileCommentRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IRepositoryBase<User> userRepository,
    IUser user,
    CultureLocalizer localizer) : IRequestHandler<CreateUserProfileCommentCommand, CreateUserProfileCommentCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserProfileComment> _userProfileCommentRepository = userProfileCommentRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<CreateUserProfileCommentCommandResponse?> Handle(CreateUserProfileCommentCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("CreateUserProfileComment", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var userProfile = _userProfileRepository.Get(x => x.Id == command.Request.UserProfileId || x.UserId == command.Request.UserProfileId);
        if (userProfile == null)
        {
            await _mediator.Publish(new DomainNotification("CreateUserProfileComment", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var userProfileComment = new UserProfileComment
        {
            UserProfileId = userProfile.Id,
            UserId = currentUser.Id,
            Comment = command.Request.Comment,
            CreatedAt = DateTime.UtcNow,
        };

        _userProfileCommentRepository.Add(userProfileComment);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("CreateUserProfileComment", _localizer.Text("Success")), cancellationToken);
        return new CreateUserProfileCommentCommandResponse
        {
            Id = userProfileComment.Id,
        };
    }
}
