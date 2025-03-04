using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.DeleteTrack;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using MediatR;

namespace Project.Application.Features.Commands.DeleteTrack
{
    public class DeleteTrackCommandHandler : IRequestHandler<DeleteTrackCommand, DeleteTrackCommandResponse?>
    {
        private readonly IMediator _mediator;
        private readonly ISupabaseService _supabaseService;
        private readonly IRepositoryBase<Track> _trackRepository;
        private readonly IRepositoryBase<Tag> _tagRepository;
        private readonly IRepositoryBase<TrackTag> _trackTagRepository;
        private readonly CultureLocalizer _localizer;
        private readonly IUser _user;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrackCommandHandler(
            IMediator mediator,
            ISupabaseService supabaseService,
            IRepositoryBase<Track> trackRepository,
            IRepositoryBase<TrackTag> trackTagRepository,
            IRepositoryBase<Tag> tagRepository,
            CultureLocalizer localizer,
            IUser user,
            IRepositoryBase<User> userRepository,
            IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _supabaseService = supabaseService;
            _trackRepository = trackRepository;
            _localizer = localizer;
            _trackTagRepository = trackTagRepository;
            _tagRepository = tagRepository;
            _user = user;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteTrackCommandResponse?> Handle(DeleteTrackCommand command, CancellationToken cancellationToken)
        {
            var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("DeleteTrack", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            var track = _trackRepository.Get(x => x.Id == command.Request.TrackId);

            if (track == null)
            {
                await _mediator.Publish(new DomainNotification("DeleteTrack", _localizer.Text("NotFound")), cancellationToken);
                return default;
            }

            if (track.UserId != user.Id)
            {
                await _mediator.Publish(new DomainNotification("DeleteTrack", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            if (!string.IsNullOrEmpty(track.ImageFileName))
            {
                await _supabaseService.DeleteFileAsync(track.ImageFileName, "jamcore-tracks");
            }
            
            if (!string.IsNullOrEmpty(track.AudioFileName))
            {
                await _supabaseService.DeleteFileAsync(track.AudioFileName, "jamcore-tracks");
            }
            
            _trackRepository.Delete(track);
            _unitOfWork.Commit();

            await _mediator.Publish(new DomainSuccessNotification("DeleteTrack", _localizer.Text("Success")), cancellationToken);
            return new DeleteTrackCommandResponse { };
        }
    }
}
