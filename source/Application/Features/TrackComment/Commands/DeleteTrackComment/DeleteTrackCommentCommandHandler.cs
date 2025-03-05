using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.DeleteTrackComment;

public class DeleteTrackCommentCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<TrackComment> trackCommentRepository, IRepositoryBase<User> userRepository, IUser user, CultureLocalizer localizer) : IRequestHandler<DeleteTrackCommentCommand, DeleteTrackCommentCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository = trackCommentRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<DeleteTrackCommentCommandResponse?> Handle(DeleteTrackCommentCommand command, CancellationToken cancellationToken)
    {
        var user = _userRepository.Get(x => x.Id == _user.Id);
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("DeleteTrackComment", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        var trackComment = _trackCommentRepository.Get(x => x.Id == command.Request.CommentId);
        if (trackComment == null)
        {
            await _mediator.Publish(new DomainNotification("DeleteTrackComment", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        if (trackComment.UserId != user.Id)
        {
            await _mediator.Publish(new DomainNotification("DeleteTrackComment", _localizer.Text("Unauthorized")), cancellationToken);
            return default;
        }

        var childComments = _trackCommentRepository.GetRanged(x => x.ParentCommentId == trackComment.Id);
        foreach (var child in childComments)
        {
            child.ParentCommentId = null;
            _trackCommentRepository.Update(child);
            _unitOfWork.Commit();
        }

        _trackCommentRepository.Delete(trackComment);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("DeleteTrackComment", _localizer.Text("Success")), cancellationToken);
        return new DeleteTrackCommentCommandResponse { };
    }

}