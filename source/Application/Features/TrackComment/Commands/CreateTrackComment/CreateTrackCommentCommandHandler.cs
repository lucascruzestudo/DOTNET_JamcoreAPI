using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateTrackComment;

public class CreateTrackCommentCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<TrackComment> trackCommentRepository, IRepositoryBase<User> userRepository, IUser user, CultureLocalizer localizer) : IRequestHandler<CreateTrackCommentCommand, CreateTrackCommentCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository = trackCommentRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<CreateTrackCommentCommandResponse?> Handle(CreateTrackCommentCommand command, CancellationToken cancellationToken)
    {

        var user = _userRepository.Get(x => x.Id == _user.Id);
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("CreateTrackComment", _localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        if (command.Request.ParentCommentId != null)
        {
            var parentComment = _trackCommentRepository.Get(x => x.Id == command.Request.ParentCommentId);
            if (parentComment == null)
            {
                await _mediator.Publish(new DomainNotification("CreateTrackComment", _localizer.Text("NotFound")), cancellationToken);
                return default;
            }
        }

        var trackComment = new TrackComment
        {
            TrackId = command.Request.TrackId,
            UserId = user.Id,
            ParentCommentId = command.Request.ParentCommentId ?? null,
            Comment = command.Request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _trackCommentRepository.Add(trackComment);
        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("CreateTrackComment", _localizer.Text("Success")), cancellationToken);
        return new CreateTrackCommentCommandResponse {
            Id = trackComment.Id
        }; 
    }
}