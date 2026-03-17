using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.DeleteUserProfileComment;

public class DeleteUserProfileCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IRepositoryBase<UserProfileComment> userProfileCommentRepository,
    IRepositoryBase<UserProfile> userProfileRepository,
    IRepositoryBase<User> userRepository,
    IUser user,
    CultureLocalizer localizer) : IRequestHandler<DeleteUserProfileCommentCommand, DeleteUserProfileCommentCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserProfileComment> _userProfileCommentRepository = userProfileCommentRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<DeleteUserProfileCommentCommandResponse?> Handle(DeleteUserProfileCommentCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("DeleteUserProfileComment", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var comment = _userProfileCommentRepository.Get(x => x.Id == command.Request.CommentId);
        if (comment == null)
        {
            await _mediator.Publish(new DomainNotification("DeleteUserProfileComment", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var targetProfile = _userProfileRepository.Get(x => x.Id == comment.UserProfileId);
        var isProfileOwner = targetProfile != null && targetProfile.UserId == currentUser.Id;

        if (comment.UserId != currentUser.Id && !isProfileOwner)
        {
            await _mediator.Publish(new DomainNotification("DeleteUserProfileComment", _localizer.Text("Unauthorized")), cancellationToken);
            return default;
        }

        _userProfileCommentRepository.Delete(comment);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("DeleteUserProfileComment", _localizer.Text("Success")), cancellationToken);
        return new DeleteUserProfileCommentCommandResponse { };
    }
}
