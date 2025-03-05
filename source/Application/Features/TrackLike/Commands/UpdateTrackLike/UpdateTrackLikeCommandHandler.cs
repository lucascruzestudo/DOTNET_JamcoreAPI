using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateTrackLike;

public class UpdateTrackLikeCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<TrackLike> trackLikeRepository, IRepositoryBase<User> userRepository, IUser user, CultureLocalizer localizer) : IRequestHandler<UpdateTrackLikeCommand, UpdateTrackLikeCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository = trackLikeRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<UpdateTrackLikeCommandResponse?> Handle(UpdateTrackLikeCommand request, CancellationToken cancellationToken)
    {

        var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("UpdateTrackLike", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }
        
        var existingTrackLike = _trackLikeRepository.Get(x => x.TrackId == request.Request.TrackId && x.UserId == user.Id);
        if (existingTrackLike == null)
        {
            var trackLike = new TrackLike
            {
                TrackId = request.Request.TrackId,
                UserId = user.Id,
            };
            _trackLikeRepository.Add(trackLike);
        }
        else
        {
            _trackLikeRepository.Delete(existingTrackLike);
        }

        _unitOfWork.Commit();

        await _mediator.Publish(new DomainSuccessNotification("UpdateTrackLike", _localizer.Text("Success")), cancellationToken);
        var response = new UpdateTrackLikeCommandResponse { };
        return response;    
    }
}