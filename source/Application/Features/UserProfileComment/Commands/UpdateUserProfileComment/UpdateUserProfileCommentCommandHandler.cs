using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateUserProfileComment;

public class UpdateUserProfileCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IRepositoryBase<UserProfileComment> userProfileCommentRepository,
    IRepositoryBase<User> userRepository,
    IUser user,
    CultureLocalizer localizer) : IRequestHandler<UpdateUserProfileCommentCommand, UpdateUserProfileCommentCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<UserProfileComment> _userProfileCommentRepository = userProfileCommentRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<UpdateUserProfileCommentCommandResponse?> Handle(UpdateUserProfileCommentCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userRepository.Get(x => x.Id == _user.Id);
        if (currentUser == null)
        {
            await _mediator.Publish(new DomainNotification("UpdateUserProfileComment", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var comment = _userProfileCommentRepository.Get(x => x.Id == command.Request.CommentId);
        if (comment == null)
        {
            await _mediator.Publish(new DomainNotification("UpdateUserProfileComment", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        if (comment.UserId != currentUser.Id)
        {
            await _mediator.Publish(new DomainNotification("UpdateUserProfileComment", _localizer.Text("Unauthorized")), cancellationToken);
            return default;
        }

        comment.Comment = command.Request.Comment;
        comment.UpdatedAt = DateTime.UtcNow;

        _userProfileCommentRepository.Update(comment);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("UpdateUserProfileComment", _localizer.Text("Success")), cancellationToken);
        return new UpdateUserProfileCommentCommandResponse { };
    }
}
