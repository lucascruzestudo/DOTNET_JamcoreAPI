using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateTrackPlay;

public class CreateTrackPlayCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<TrackPlay> trackPlayRepository, IRepositoryBase<User> userRepository, IUser user, CultureLocalizer localizer) : IRequestHandler<CreateTrackPlayCommand, CreateTrackPlayCommandResponse?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository = trackPlayRepository;
    private readonly IRepositoryBase<User> _userRepository = userRepository;
    private readonly IUser _user = user;
    private readonly CultureLocalizer _localizer = localizer;

    public async Task<CreateTrackPlayCommandResponse?> Handle(CreateTrackPlayCommand request, CancellationToken cancellationToken)
    {

        var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("CreateTrackPlay", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }
        
        var latestTrackPlay = _trackPlayRepository.GetRanged(tp => tp.UserId == user.Id && tp.TrackId == request.Request.TrackId).OrderByDescending(tp => tp.CreatedAt).FirstOrDefault();

        if (latestTrackPlay == null || DateTime.Now - latestTrackPlay.CreatedAt >= TimeSpan.FromSeconds(30))
        {
            var trackPlay = new TrackPlay
            {
                TrackId = request.Request.TrackId,
                UserId = user.Id
            };
            _trackPlayRepository.Add(trackPlay);
            _unitOfWork.Commit();
        }

        await _mediator.Publish(new DomainSuccessNotification("CreateTrackPlay", _localizer.Text("Success")), cancellationToken);
        var response = new CreateTrackPlayCommandResponse { };
        return response;    
    }
}